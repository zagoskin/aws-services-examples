namespace Customers.Consumer;

internal class QueueSettings
{
    public const string SectionName = "QueueSettings";
    public required string QueueName { get; init; }
}
