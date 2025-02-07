namespace _06_ServiceBus.Config;

public record AzureServiceBusConfig
{
    public const string SectionName = "AzureServiceBus";

    public string ConnectionString { get; init; } = string.Empty;

    public string QueueName { get; init; } = string.Empty;

    public string TopicName { get; init; } = string.Empty;

    public string SubscriptionName { get; init; } = string.Empty;
}