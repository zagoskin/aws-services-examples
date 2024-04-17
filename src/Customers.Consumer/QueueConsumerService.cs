using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer;
using Customers.Contracts;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

internal sealed class QueueConsumerService : BackgroundService
{
    private static readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<QueueConsumerService> _logger;
    private readonly IAmazonSQS _sqs;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly QueueSettings _queueSettings;
    private string? _queueUrl;
    public QueueConsumerService(
        ILogger<QueueConsumerService> logger,
        IAmazonSQS sqs,
        IOptions<QueueSettings> options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _sqs = sqs;
        _serviceScopeFactory = serviceScopeFactory;
        _queueSettings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);
        var queueUrl = await GetQueueUrlAsync(stoppingToken);
        var receiveMessageRequest = new ReceiveMessageRequest(queueUrl)
        {
            AttributeNames = ["All"],
            MessageAttributeNames = ["All"],
            MaxNumberOfMessages = 1
        };

        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ReceiveMessages(receiveMessageRequest, stoppingToken);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
            }
        }
    }

    private async Task ReceiveMessages(ReceiveMessageRequest receiveMessageRequest, CancellationToken stoppingToken)
    {
        var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
        if (!response.IsSuccessResponse())
        {
            _logger.LogError("Failed to receive messages from queue: {QueueName}", _queueSettings.QueueName);
            return;
        }

        foreach (var message in response.Messages)
        {
            await PublishAsync(message, stoppingToken);            
        }

    }    

    private async Task PublishAsync(Message message, CancellationToken token = default)
    {
        _logger.LogInformation("Message received: {Message}", message.Body);
        if (!message.MessageAttributes.TryGetValue("MessageType", out var messageType))
        {
            _logger.LogError("Message does not have MessageType attribute. Continuing...");
            return;
        }

        var contractsAssembly = Assembly.GetAssembly(typeof(IIntegrationEvent));
        var type = contractsAssembly?.GetTypes().FirstOrDefault(t => t.Name == messageType.StringValue);
        if (type is null)
        {
            _logger.LogWarning("Unknown Type {Type}", messageType.StringValue);
            return;
        }

        var contract = Deserialize(message.Body, type) as IIntegrationEvent;
        if (contract is null)
        {
            _logger.LogError("Failed to deserialize message body");
            return;
        }

        _logger.LogInformation("Publishing event of Type {Type}.", messageType.StringValue);
        using var eventScope = _serviceScopeFactory.CreateScope();
        var publisher = eventScope.ServiceProvider.GetRequiredService<IPublisher>();
        try
        {
            await publisher.Publish(contract, token);
            await _sqs.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, token);
            _logger.LogInformation("Event of Type {Type} published", messageType.StringValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event of Type {Type}", messageType.StringValue);
        }
    }

    private async Task<string> GetQueueUrlAsync(CancellationToken token = default)
    {
        if (_queueUrl is not null)
        {
            return _queueUrl;
        }

        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.QueueName, token);
        if (queueUrlResponse.HttpStatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("Queue not found: {QueueName}", _queueSettings.QueueName);
            throw new InvalidOperationException("Queue not found");
        }

        _queueUrl = queueUrlResponse.QueueUrl;
        return _queueUrl;
    }

    private static T Deserialize<T>(string body)
    {
        return (T)Deserialize(body, typeof(T));
    }

    private static object Deserialize(string body, Type type)
    {
        return JsonSerializer.Deserialize(body, type, _jsonSerializerOptions) ?? throw new InvalidOperationException("Failed to deserialize message body");
    }


}
