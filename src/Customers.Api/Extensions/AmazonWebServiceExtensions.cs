using Amazon.Runtime;
using System.Net;

namespace Customers.Api.Extensions;

internal static class AmazonWebServiceExtensions
{
    internal static bool IsSuccessResponse(this AmazonWebServiceResponse response)
    {
        return response.HttpStatusCode switch
        {
            >= HttpStatusCode.OK and < HttpStatusCode.Ambiguous => true,
            _ => false
        };
    }
}
