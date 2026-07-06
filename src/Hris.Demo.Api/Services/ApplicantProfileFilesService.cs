using Hris.Demo.Api.Configuration;
using Hris.Demo.Api.Data;
using Hris.Demo.Shared.ApplicantFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hris.Demo.Api.Services;

public sealed class ApplicantProfileFilesService(
    AppDbContext db,
    MockRspStore rsp,
    IObjectStorageService storage,
    IOptionsMonitor<StorageOptions> storageOptions)
{
    public bool StorageReady => storage.IsConfigured;

    public ApplicantFileUploadUrlResponse? TryCreateUploadUrl(
        Guid applicantId,
        ApplicantFileUploadUrlRequest request,
        out string? errorMessage,
        out int statusCode)
    {
        errorMessage = null;
        statusCode = StatusCodes.Status200OK;

        if (!storage.IsConfigured)
        {
            statusCode = StatusCodes.Status503ServiceUnavailable;
            errorMessage = "File uploads are disabled: S3 storage is not configured on the server.";
            return null;
        }

        if (!rsp.ApplicantExists(applicantId))
        {
            statusCode = StatusCodes.Status404NotFound;
            errorMessage = "Applicant not found.";
            return null;
        }

        if (!ValidateCategoryIntent(request, out errorMessage))
        {
            statusCode = StatusCodes.Status400BadRequest;
            return null;
        }

        var opt = storageOptions.CurrentValue;
        var fileId = Guid.NewGuid();
        var safeName = ApplicantFileNameSanitizer.Sanitize(request.FileName);
        var categoryName = request.Category.ToString();
        var objectKey = $"applicants/{applicantId:N}/{categoryName}/{fileId:D}-{safeName}";
        var contentType = ApplicantFilePolicy.NormalizeContentType(request.ContentType);
        var lifetime = TimeSpan.FromMinutes(Math.Clamp(opt.SignedUploadMinutes, 5, 60));

        var uploadUrl = storage.CreateUploadPutUrl(objectKey, contentType, lifetime);
        var expires = DateTimeOffset.UtcNow.Add(lifetime);

        return new ApplicantFileUploadUrlResponse
        {
            UploadUrl = uploadUrl,
            ObjectKey = objectKey,
            RequiredHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Content-Type"] = contentType,
            },
            ExpiresAtUtc = expires,
        };
    }

    public async Task<ApplicantFileServiceResult<ApplicantFileMetadataDto>> CompleteAsync(
        Guid applicantId,
        ApplicantFileCompleteRequest request,
        CancellationToken cancellationToken)
    {
        if (!storage.IsConfigured)
        {
            return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail(
                "File uploads are disabled: S3 storage is not configured on the server.",
                StatusCodes.Status503ServiceUnavailable);
        }

        if (!rsp.ApplicantExists(applicantId))
        {
            return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail("Applicant not found.", StatusCodes.Status404NotFound);
        }

        var expectedPrefix = $"applicants/{applicantId:N}/";
        if (!request.ObjectKey.StartsWith(expectedPrefix, StringComparison.Ordinal))
        {
            return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail(
                "Object key does not belong to this applicant.",
                StatusCodes.Status400BadRequest);
        }

        var category = request.Category;

        var declaredType = ApplicantFilePolicy.NormalizeContentType(request.ContentType);
        if (!IsAllowedContentType(category, declaredType))
        {
            return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail(
                "Unsupported content type for this category.",
                StatusCodes.Status400BadRequest);
        }

        ObjectHeadMetadata head;
        try
        {
            head = await storage.GetObjectMetadataAsync(request.ObjectKey, cancellationToken).ConfigureAwait(false);
        }
        catch (FileNotFoundException)
        {
            return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail(
                "Upload not found in storage yet. Wait for the direct upload to finish, then try again.",
                StatusCodes.Status400BadRequest);
        }

        var headType = head.ContentType is null ? declaredType : ApplicantFilePolicy.NormalizeContentType(head.ContentType);
        if (!IsAllowedContentType(category, headType))
        {
            await storage.DeleteObjectAsync(request.ObjectKey, cancellationToken).ConfigureAwait(false);
            return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail(
                "Stored object has an unsupported content type.",
                StatusCodes.Status400BadRequest);
        }

        var opt = storageOptions.CurrentValue;
        var maxImage = opt.MaxImageBytes > 0 ? opt.MaxImageBytes : ApplicantFilePolicy.MaxServerImageBytes;
        var maxPdf = opt.MaxPdfBytes > 0 ? opt.MaxPdfBytes : ApplicantFilePolicy.MaxPdfBytes;

        if (IsImageCategory(category))
        {
            if (head.ContentLength > maxImage)
            {
                await storage.DeleteObjectAsync(request.ObjectKey, cancellationToken).ConfigureAwait(false);
                return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail(
                    $"Image exceeds server limit of {maxImage} bytes after upload.",
                    StatusCodes.Status400BadRequest);
            }
        }
        else if (IsPdfCategory(category))
        {
            if (head.ContentLength > maxPdf)
            {
                await storage.DeleteObjectAsync(request.ObjectKey, cancellationToken).ConfigureAwait(false);
                return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Fail(
                    $"PDF exceeds maximum size of {maxPdf} bytes.",
                    StatusCodes.Status400BadRequest);
            }
        }

        var categoryStr = category.ToString();
        var now = DateTimeOffset.UtcNow;

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var previous = await db.ApplicantFiles
                .Where(f => f.ApplicantId == applicantId && f.Category == categoryStr && f.IsActive)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var p in previous)
            {
                p.IsActive = false;
            }

            var entity = new ApplicantFileRecord
            {
                Id = Guid.NewGuid(),
                ApplicantId = applicantId,
                Category = categoryStr,
                ObjectKey = request.ObjectKey,
                OriginalFileName = ApplicantFileNameSanitizer.Sanitize(request.OriginalFileName),
                ContentType = headType,
                SizeBytes = head.ContentLength,
                StorageProvider = "S3",
                UploadedAtUtc = now,
                IsActive = true,
            };

            db.ApplicantFiles.Add(entity);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await tx.CommitAsync(cancellationToken).ConfigureAwait(false);

            _ = DeleteOldObjectsAsync(previous.Select(p => p.ObjectKey).Where(k => k != request.ObjectKey).ToList());

            return ApplicantFileServiceResult<ApplicantFileMetadataDto>.Ok(ToDto(entity));
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    private async Task DeleteOldObjectsAsync(List<string> keys)
    {
        if (!storage.IsConfigured || keys.Count == 0)
        {
            return;
        }

        foreach (var key in keys)
        {
            try
            {
                await storage.DeleteObjectAsync(key, CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                /* best-effort cleanup */
            }
        }
    }

    public async Task<IReadOnlyList<ApplicantFileMetadataDto>?> ListAsync(Guid applicantId, CancellationToken cancellationToken)
    {
        if (!rsp.ApplicantExists(applicantId))
        {
            return null;
        }

        // SQLite provider cannot translate ORDER BY DateTimeOffset — materialize then sort in memory.
        var rows = await db.ApplicantFiles
            .AsNoTracking()
            .Where(f => f.ApplicantId == applicantId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var ordered = rows.OrderByDescending(f => f.UploadedAtUtc).ToList();
        return ordered.Select(ToDto).ToList();
    }

    public async Task<ApplicantFileServiceResult<ApplicantFileDownloadUrlResponse>> GetDownloadUrlAsync(
        Guid applicantId,
        Guid fileId,
        CancellationToken cancellationToken)
    {
        if (!storage.IsConfigured)
        {
            return ApplicantFileServiceResult<ApplicantFileDownloadUrlResponse>.Fail(
                "Downloads are unavailable: S3 storage is not configured on the server.",
                StatusCodes.Status503ServiceUnavailable);
        }

        var file = await db.ApplicantFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId && f.ApplicantId == applicantId, cancellationToken)
            .ConfigureAwait(false);

        if (file is null)
        {
            return ApplicantFileServiceResult<ApplicantFileDownloadUrlResponse>.Fail("File not found.", StatusCodes.Status404NotFound);
        }

        if (!file.IsActive)
        {
            return ApplicantFileServiceResult<ApplicantFileDownloadUrlResponse>.Fail(
                "File is no longer available.",
                StatusCodes.Status404NotFound);
        }

        var opt = storageOptions.CurrentValue;
        var lifetime = TimeSpan.FromMinutes(Math.Clamp(opt.SignedDownloadMinutes, 1, 120));
        var url = storage.CreateDownloadGetUrl(file.ObjectKey, lifetime);
        var body = new ApplicantFileDownloadUrlResponse
        {
            Url = url,
            ExpiresAtUtc = DateTimeOffset.UtcNow.Add(lifetime),
        };

        return ApplicantFileServiceResult<ApplicantFileDownloadUrlResponse>.Ok(body);
    }

    public async Task<ApplicantFileServiceResult<object?>> DeleteAsync(
        Guid applicantId,
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var file = await db.ApplicantFiles.FirstOrDefaultAsync(f => f.Id == fileId && f.ApplicantId == applicantId, cancellationToken).ConfigureAwait(false);
        if (file is null)
        {
            return ApplicantFileServiceResult<object?>.Fail("File not found.", StatusCodes.Status404NotFound);
        }

        file.IsActive = false;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (storage.IsConfigured)
        {
            try
            {
                await storage.DeleteObjectAsync(file.ObjectKey, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                /* metadata already soft-deleted */
            }
        }

        return new ApplicantFileServiceResult<object?>(null, null, StatusCodes.Status204NoContent);
    }

    private bool ValidateCategoryIntent(ApplicantFileUploadUrlRequest request, out string? errorMessage)
    {
        errorMessage = null;
        var ct = ApplicantFilePolicy.NormalizeContentType(request.ContentType);

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            errorMessage = "File name is required.";
            return false;
        }

        if (request.SizeBytes <= 0)
        {
            errorMessage = "Size must be greater than zero.";
            return false;
        }

        switch (request.Category)
        {
            case ApplicantFileCategory.Avatar:
            case ApplicantFileCategory.Cover:
                if (!ApplicantFilePolicy.IsAllowedImageContentType(ct))
                {
                    errorMessage = "Images must be JPEG, PNG, or WebP.";
                    return false;
                }

                if (request.SizeBytes > ApplicantFilePolicy.MaxServerImageBytes)
                {
                    errorMessage = $"Compressed image intent exceeds server maximum of {ApplicantFilePolicy.MaxServerImageBytes} bytes.";
                    return false;
                }

                return true;

            case ApplicantFileCategory.ResumePdf:
            case ApplicantFileCategory.CvPdf:
                if (!ApplicantFilePolicy.IsPdfContentType(ct))
                {
                    errorMessage = "Resume and CV uploads must be PDF (application/pdf).";
                    return false;
                }

                if (request.SizeBytes > ApplicantFilePolicy.MaxPdfBytes)
                {
                    errorMessage = $"PDF exceeds maximum size of {ApplicantFilePolicy.MaxPdfBytes} bytes.";
                    return false;
                }

                return true;

            default:
                errorMessage = "Unknown file category.";
                return false;
        }
    }

    private static bool IsImageCategory(ApplicantFileCategory c) =>
        c is ApplicantFileCategory.Avatar or ApplicantFileCategory.Cover;

    private static bool IsPdfCategory(ApplicantFileCategory c) =>
        c is ApplicantFileCategory.ResumePdf or ApplicantFileCategory.CvPdf;

    private static bool IsAllowedContentType(ApplicantFileCategory category, string normalizedContentType) =>
        IsImageCategory(category)
            ? ApplicantFilePolicy.IsAllowedImageContentType(normalizedContentType)
            : ApplicantFilePolicy.IsPdfContentType(normalizedContentType);

    private static ApplicantFileMetadataDto ToDto(ApplicantFileRecord r) =>
        new()
        {
            Id = r.Id,
            ApplicantId = r.ApplicantId,
            Category = Enum.Parse<ApplicantFileCategory>(r.Category, ignoreCase: true),
            ObjectKey = r.ObjectKey,
            OriginalFileName = r.OriginalFileName,
            ContentType = r.ContentType,
            SizeBytes = r.SizeBytes,
            StorageProvider = r.StorageProvider,
            UploadedAtUtc = r.UploadedAtUtc,
            IsActive = r.IsActive,
        };
}
