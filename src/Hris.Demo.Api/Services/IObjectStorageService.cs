namespace Hris.Demo.Api.Services;

public interface IObjectStorageService
{
    bool IsConfigured { get; }

    string CreateUploadPutUrl(string objectKey, string contentType, TimeSpan lifetime);

    string CreateDownloadGetUrl(string objectKey, TimeSpan lifetime);

    Task<ObjectHeadMetadata> GetObjectMetadataAsync(string objectKey, CancellationToken cancellationToken);

    Task DeleteObjectAsync(string objectKey, CancellationToken cancellationToken);
}

public sealed record ObjectHeadMetadata(long ContentLength, string? ContentType);
