using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using System.Net.Mime;
using System.Text.Json;

namespace Customers.Api.Messaging;

internal sealed class SnsMessenger : ISnsMessenger
{
    private readonly IAmazonSimpleNotificationService _amazonSNS;
    private readonly ILogger<SnsMessenger> _logger;
    private readonly TopicSettings _topicSettings;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private string? _topicArn;
    public SnsMessenger(IAmazonSimpleNotificationService amazonSNS, IOptions<TopicSettings> options, ILogger<SnsMessenger> logger)
    {
        _amazonSNS = amazonSNS;
        _logger = logger;
        _topicSettings = options.Value;
    }
    public async Task<PublishResponse> PublishMessageAsync<T>(T message, CancellationToken token = default)
    {        
        var serializedMessage = JsonSerializer.Serialize(message, _jsonSerializerOptions);

        _logger.LogInformation("Sending message to topic {TopicName}. Message body: {MessageBody}", _topicSettings.TopicName, serializedMessage);
        var topicArn = await GetTopicArnAsync(token);
        _logger.LogInformation("Topic URL: {TopicUrl}", topicArn);

        var sendMessageRequest = new PublishRequest(topicArn, serializedMessage)
            .WithMessageType(typeof(T))
            .WithContentType(MediaTypeNames.Application.Json);

        var response = await _amazonSNS.PublishAsync(sendMessageRequest, token);
        if (!response.IsSuccessResponse())
        {
            _logger.LogError("Failed to send message to topic {TopicName}. Response body: {MessageBody}", _topicSettings.TopicName, JsonSerializer.Serialize(response, _jsonSerializerOptions));
            throw new InvalidOperationException("Failed to send message to topic");
        }

        _logger.LogInformation("Message sent to topic {TopicName}. Message ID: {MessageId}", _topicSettings.TopicName, response.MessageId);
        return response;
    }

    private async ValueTask<string> GetTopicArnAsync(CancellationToken token = default)
    {
        if (_topicArn is not null)
        {
            return _topicArn;
        }

        var topic = await _amazonSNS.FindTopicAsync(_topicSettings.TopicName); 
        if (topic is null)
        {
            _logger.LogError("Topic not found: {TopicName}", _topicSettings.TopicName);
            throw new InvalidOperationException("Topic not found");
        }

        _topicArn = topic.TopicArn;
        return _topicArn;
    }
}