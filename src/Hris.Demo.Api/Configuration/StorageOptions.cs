using Hris.Demo.Shared.ApplicantFiles;

namespace Hris.Demo.Api.Configuration;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>When set to <c>S3</c> and credentials are present, object storage is enabled.</summary>
    public string Provider { get; set; } = string.Empty;

    public StorageS3Options S3 { get; set; } = new();

    /// <summary>Maximum stored image size (bytes) after upload; defaults to shared policy.</summary>
    public long MaxImageBytes { get; set; } = ApplicantFilePolicy.MaxServerImageBytes;

    public long MaxPdfBytes { get; set; } = ApplicantFilePolicy.MaxPdfBytes;

    public int SignedUploadMinutes { get; set; } = 15;

    public int SignedDownloadMinutes { get; set; } = 15;
}

public sealed class StorageS3Options
{
    public string? BucketName { get; set; }

    public string? Region { get; set; }

    public string? AccessKeyId { get; set; }

    public string? SecretAccessKey { get; set; }

    public bool UsePathStyle { get; set; }
}
