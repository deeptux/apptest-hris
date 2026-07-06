namespace Hris.Demo.Shared.ApplicantFiles;

public sealed class ApplicantFileUploadUrlRequest
{
    public ApplicantFileCategory Category { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    /// <summary>Declared byte length the client will PUT to object storage (post-compression for images).</summary>
    public long SizeBytes { get; set; }
}

public sealed class ApplicantFileUploadUrlResponse
{
    public string UploadUrl { get; set; } = string.Empty;

    public string ObjectKey { get; set; } = string.Empty;

    public Dictionary<string, string> RequiredHeaders { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public DateTimeOffset ExpiresAtUtc { get; set; }
}

public sealed class ApplicantFileCompleteRequest
{
    public string ObjectKey { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public ApplicantFileCategory Category { get; set; }
}

public sealed class ApplicantFileMetadataDto
{
    public Guid Id { get; set; }

    public Guid ApplicantId { get; set; }

    public ApplicantFileCategory Category { get; set; }

    public string ObjectKey { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public string StorageProvider { get; set; } = string.Empty;

    public DateTimeOffset UploadedAtUtc { get; set; }

    public bool IsActive { get; set; }
}

public sealed class ApplicantFileDownloadUrlResponse
{
    public string Url { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; set; }
}
