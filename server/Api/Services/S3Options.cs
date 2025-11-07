namespace Api;

public class S3Options
{
    public string AwsAccessKeyId { get; set; } = null!;
    public string AwsEndpointUrlS3 { get; set; } = null!;
    public string AwsSecretAccessKey { get; set; } = null!;
    public string BucketName { get; set; } = null!;
}