using Amazon;
using Amazon.S3;
using Hris.Demo.Api.Services;

namespace Hris.Demo.Api.Configuration;

public static class StorageRegistrar
{
    public static void AddObjectStorage(WebApplicationBuilder builder)
    {
        builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection(StorageOptions.SectionName));
        var snapshot = builder.Configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();

        if (TryCreateS3Client(snapshot, out var client))
        {
            builder.Services.AddSingleton(client);
            builder.Services.AddSingleton<IObjectStorageService, S3ObjectStorageService>();
        }
        else
        {
            builder.Services.AddSingleton<IObjectStorageService, UnconfiguredObjectStorageService>();
        }
    }

    private static bool TryCreateS3Client(StorageOptions options, out IAmazonS3 client)
    {
        client = null!;
        if (!string.Equals(options.Provider, "S3", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var s3 = options.S3;
        if (s3 is null
            || string.IsNullOrWhiteSpace(s3.BucketName)
            || string.IsNullOrWhiteSpace(s3.Region)
            || string.IsNullOrWhiteSpace(s3.AccessKeyId)
            || string.IsNullOrWhiteSpace(s3.SecretAccessKey))
        {
            return false;
        }

        var cfg = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(s3.Region.Trim()),
            ForcePathStyle = s3.UsePathStyle,
        };

        client = new AmazonS3Client(s3.AccessKeyId.Trim(), s3.SecretAccessKey.Trim(), cfg);
        return true;
    }
}
