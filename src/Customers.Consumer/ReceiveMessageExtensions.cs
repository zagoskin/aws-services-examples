using Amazon.SQS.Model;

internal static class ReceiveMessageExtensions
{
    internal static bool IsSuccessResponse(this ReceiveMessageResponse response)
    {
        return (int)response.HttpStatusCode switch
        {
            >= 200 and < 300 => true,
            _ => false
        };
    }
}