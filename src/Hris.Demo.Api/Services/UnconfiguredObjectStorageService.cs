namespace Hris.Demo.Api.Services;

/// <summary>Placeholder when S3 credentials are not supplied (local dev without uploads).</summary>
public sealed class UnconfiguredObjectStorageService : IObjectStorageService
{
    public bool IsConfigured => false;

    public string CreateUploadPutUrl(string objectKey, string contentType, TimeSpan lifetime) =>
        throw new InvalidOperationException("Object storage is not configured.");

    public string CreateDownloadGetUrl(string objectKey, TimeSpan lifetime) =>
        throw new InvalidOperationException("Object storage is not configured.");

    public Task<ObjectHeadMetadata> GetObjectMetadataAsync(string objectKey, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("Object storage is not configured.");

    public Task DeleteObjectAsync(string objectKey, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
