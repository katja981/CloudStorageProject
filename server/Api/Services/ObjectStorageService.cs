using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class ObjectStorageService
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;

    public ObjectStorageService(IOptions<S3Options> options)
    {
        var opt = options.Value;
        _bucketName = opt.BucketName;

        _s3Client = new AmazonS3Client(
            opt.AwsAccessKeyId,
            opt.AwsSecretAccessKey,
            new AmazonS3Config
            {
                ServiceURL = opt.AwsEndpointUrlS3,
                UseHttp = false
            }
        );
    }

    public async Task UploadAsync(string key, Stream content)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = content,
            UseChunkEncoding = false
        };
        await _s3Client.PutObjectAsync(request);
    }

    public async Task<Stream> DownloadAsync(string key)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };
        var response = await _s3Client.GetObjectAsync(request);
        return response.ResponseStream;
    }

    public async Task DeleteAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };
        await _s3Client.DeleteObjectAsync(request);
    }
}