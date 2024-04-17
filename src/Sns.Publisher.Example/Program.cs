// See https://aka.ms/new-console-template for more information
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Bogus;
using Customers.Contracts;
using System.Text.Json;

var customerCreated = new Faker<CustomerCreated>()
    .CustomInstantiator(f => new CustomerCreated(
        Guid.NewGuid(),
        f.Person.FullName,
        f.Person.Email,
        "zagoskin",
        f.Person.DateOfBirth));

var snsClient = new AmazonSimpleNotificationServiceClient();

var topic = await snsClient.FindTopicAsync("dome-customers");

if (topic is null)
{
    Console.WriteLine("Topic not found");
    return;
}

var publishRequest = new PublishRequest
{
    TopicArn = topic.TopicArn,
    Message = JsonSerializer.Serialize(customerCreated),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
            "MessageType",
            new MessageAttributeValue
            {
                DataType = nameof(String),
                StringValue = nameof(CustomerCreated)
            }
        }
    }
};

var response = await snsClient.PublishAsync(publishRequest);
Console.WriteLine($"Status: {response.HttpStatusCode}");
