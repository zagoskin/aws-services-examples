using Amazon.SimpleNotificationService.Model;

namespace Customers.Api.Messaging;

internal static class SendMessageExtensions
{
    internal static PublishRequest WithMessageType(this PublishRequest request, Type type)
    {
        const string MessageTypeKey = "MessageType";        
        return request.WithStringValue(MessageTypeKey, type.Name);
    }

    internal static PublishRequest WithContentType(this PublishRequest request, string contentType)
    {
        const string ContentTypeKey = "ContentType";
        return request.WithStringValue(ContentTypeKey, contentType);
    }

    internal static bool IsSuccessResponse(this PublishResponse response)
    {
        return (int)response.HttpStatusCode switch
        {
            >= 200 and < 300 => true,
            _ => false
        };
    }

    private static PublishRequest WithStringValue(this PublishRequest request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(nameof(request));        
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("A key is required", nameof(key));
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("A value is required", nameof(value));
        }

        request.MessageAttributes ??= new();

        if (request.MessageAttributes.ContainsKey(key))
        {
            throw new InvalidOperationException($"{key} is already defined");
        }

        request.MessageAttributes.Add(key, new()
        {
            DataType = nameof(String),
            StringValue = value
        });

        return request;
    }
}