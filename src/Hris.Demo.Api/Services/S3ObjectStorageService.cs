using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Hris.Demo.Api.Configuration;
using Microsoft.Extensions.Options;

namespace Hris.Demo.Api.Services;

public sealed class S3ObjectStorageService(IAmazonS3 s3, IOptionsMonitor<StorageOptions> options) : IObjectStorageService
{
    public bool IsConfigured => true;

    private string Bucket => options.CurrentValue.S3.BucketName
        ?? throw new InvalidOperationException("Storage:S3:BucketName is required.");

    public string CreateUploadPutUrl(string objectKey, string contentType, TimeSpan lifetime)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = Bucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.Add(lifetime),
            ContentType = contentType,
        };

        return s3.GetPreSignedURL(request);
    }

    public string CreateDownloadGetUrl(string objectKey, TimeSpan lifetime)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = Bucket,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(lifetime),
        };

        return s3.GetPreSignedURL(request);
    }

    public async Task<ObjectHeadMetadata> GetObjectMetadataAsync(string objectKey, CancellationToken cancellationToken)
    {
        try
        {
            var response = await s3.GetObjectMetadataAsync(Bucket, objectKey, cancellationToken).ConfigureAwait(false);
            var ct = response.Headers.ContentType;
            return new ObjectHeadMetadata(response.ContentLength, string.IsNullOrWhiteSpace(ct) ? null : ct);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException("Object not found in storage.", objectKey, ex);
        }
    }

    public Task DeleteObjectAsync(string objectKey, CancellationToken cancellationToken) =>
        s3.DeleteObjectAsync(Bucket, objectKey, cancellationToken);
}
