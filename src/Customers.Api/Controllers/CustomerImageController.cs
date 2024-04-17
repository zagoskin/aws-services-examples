using Amazon.S3;
using Customers.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Customers.Api.Controllers;

[ApiController]
public class CustomerIMageController : ControllerBase
{
    private readonly ICustomerImageService _customerImageService;

    public CustomerIMageController(ICustomerImageService customerImageService)
    {
        _customerImageService = customerImageService;
    }


    [HttpPost("customers/{customerId:guid}/image")]
    public async Task<IActionResult> UploadImageAsync(Guid customerId, IFormFile file)
    {
        if (file is null)
        {
            return BadRequest("File is required");
        }

        var response = await _customerImageService.UploadImageAsync(customerId, file);
        if (response.HttpStatusCode is not HttpStatusCode.OK)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpGet("customers/{customerId:guid}/image")]
    public async Task<IActionResult> GetImageAsync(Guid customerId)
    {
        try
        {
            var response = await _customerImageService.GetImageAsync(customerId);
            if (response.HttpStatusCode is not HttpStatusCode.OK)
            {
                return BadRequest();
            }

            return File(response.ResponseStream, response.Headers.ContentType);
        }
        catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist.")
        {
            return NotFound();
        }
    }

    [HttpDelete("customers/{customerId:guid}/image")]
    public async Task<IActionResult> DeleteImageAsync(Guid customerId)
    {
        try
        {
            var response = await _customerImageService.DeleteImageAsync(customerId);
            return response.HttpStatusCode switch
            {
                HttpStatusCode.OK => Ok(),
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.BadRequest => BadRequest(),
                HttpStatusCode.Unauthorized => Unauthorized(),
                HttpStatusCode.Forbidden => Forbid(),
                _ => Problem()
            };
        }
        catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist")
        {
            return NotFound();
        }
    }
}
