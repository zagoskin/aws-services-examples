using Amazon.S3.Model;

namespace Customers.Api.Services;

public interface ICustomerImageService
{
    Task<PutObjectResponse> UploadImageAsync(Guid customerId, IFormFile file);
    Task<GetObjectResponse> GetImageAsync(Guid customerId);
    Task<DeleteObjectResponse> DeleteImageAsync(Guid customerId);
}
