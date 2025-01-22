namespace _03_AzureFunction_QueueTrigger.Services.Interfaces;

public interface IBlobStorageService
{
    Task AppendToBlobAsync(string containerName, string blobName, string content);
}
