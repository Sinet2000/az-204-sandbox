namespace _03_AzureFunction_QueueTrigger.Messaging;

public record CreateUserEventMessage(string EventName, string ExternalUserID);