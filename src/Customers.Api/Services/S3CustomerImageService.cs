using Amazon.S3;
using Amazon.S3.Model;

namespace Customers.Api.Services;

internal sealed class S3CustomerImageService : ICustomerImageService
{
    private readonly IAmazonS3 _amazonS3;
    private readonly string _bucketName = "dome-course";
    public S3CustomerImageService(IAmazonS3 amazonS3)
    {
        _amazonS3 = amazonS3;
    }
    public async Task<DeleteObjectResponse> DeleteImageAsync(Guid customerId)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = $"customers/{customerId}"
        };

        return await _amazonS3.DeleteObjectAsync(deleteObjectRequest);
    }

    public async Task<GetObjectResponse> GetImageAsync(Guid customerId)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = $"customers/{customerId}"
        };

        return await _amazonS3.GetObjectAsync(getObjectRequest);
    }

    public async Task<PutObjectResponse> UploadImageAsync(Guid customerId, IFormFile file)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = $"customers/{customerId}",
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = file.FileName,
                ["x-amz-meta-extension"] = Path.GetExtension(file.FileName),
            }
        };

        return await _amazonS3.PutObjectAsync(putObjectRequest);
    }
}
