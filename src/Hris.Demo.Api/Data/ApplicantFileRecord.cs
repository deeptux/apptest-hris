namespace Hris.Demo.Api.Data;

public sealed class ApplicantFileRecord
{
    public Guid Id { get; set; }

    public Guid ApplicantId { get; set; }

    /// <summary>Serialized <see cref="Hris.Demo.Shared.ApplicantFiles.ApplicantFileCategory"/> name.</summary>
    public string Category { get; set; } = string.Empty;

    public string ObjectKey { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public string StorageProvider { get; set; } = "S3";

    public DateTimeOffset UploadedAtUtc { get; set; }

    public bool IsActive { get; set; } = true;
}
