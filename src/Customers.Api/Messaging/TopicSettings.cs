namespace Customers.Api.Messaging;

internal class TopicSettings
{
    public const string SectionName = "TopicSettings";
    public required string TopicName { get; init; }
}
