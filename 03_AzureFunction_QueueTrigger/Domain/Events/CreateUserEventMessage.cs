namespace _03_AzureFunction_QueueTrigger.Domain.Events;

public record CreateUserEventMessage(string EventName, string ExternalUserID);