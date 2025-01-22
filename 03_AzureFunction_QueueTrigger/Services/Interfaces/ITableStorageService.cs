using Azure.Data.Tables;

namespace _03_AzureFunction_QueueTrigger.Services.Interfaces;

public interface ITableStorageService
{
    Task SaveEntityAsync(string tableName, TableEntity entity, string rowKey);
}