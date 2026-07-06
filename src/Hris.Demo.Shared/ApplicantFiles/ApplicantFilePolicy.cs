namespace Hris.Demo.Shared.ApplicantFiles;

/// <summary>
/// Shared validation limits for applicant profile uploads (v1).
/// Server enforces these regardless of client checks.
/// </summary>
public static class ApplicantFilePolicy
{
    /// <summary>Maximum bytes read from disk for an image before client-side compression.</summary>
    public const long MaxRawImagePickerBytes = 10 * 1024 * 1024;

    /// <summary>Target maximum for compressed image bytes before requesting a signed upload URL.</summary>
    public const long TargetCompressedImageBytes = 1024 * 1024;

    /// <summary>Hard cap after upload (HEAD); oversize objects are rejected and deleted from S3.</summary>
    public const long MaxServerImageBytes = (long)(1.5 * 1024 * 1024);

    public const long MaxPdfBytes = 10 * 1024 * 1024;

    /// <summary>Long edge cap after resize (profile imagery).</summary>
    public const int MaxImageLongEdgePixels = 1536;

    public static readonly string[] AllowedImageContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public const string PdfContentType = "application/pdf";

    public static bool IsAllowedImageContentType(string contentType) =>
        AllowedImageContentTypes.Contains(NormalizeContentType(contentType), StringComparer.OrdinalIgnoreCase);

    public static bool IsPdfContentType(string contentType) =>
        string.Equals(NormalizeContentType(contentType), PdfContentType, StringComparison.OrdinalIgnoreCase);

    public static string NormalizeContentType(string contentType) => contentType.Trim().ToLowerInvariant();
}
