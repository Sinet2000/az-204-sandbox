using System.Text;
using System.Text.Json;
using _03_AzureFunction_QueueTrigger.Data;
using _03_AzureFunction_QueueTrigger.Data.Entities;
using _03_AzureFunction_QueueTrigger.Messaging;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Dapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace _03_AzureFunction_QueueTrigger;

public class Az3QueueTrigger(
    ILoggerFactory loggerFactory,
    IDbConnectionFactory dbConnectionFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<Az3QueueTrigger>();
    private const string PartitionKey = "03AZF";

    [Function(nameof(Az3QueueTrigger))]
    public async Task Run([QueueTrigger("az-204-queue-items")] string queueItem)
    {
        _logger.LogInformation("C# Queue trigger function processed: {QueueItem}", queueItem);

        await InitializeDatabaseAsync();
        await ProcessQueueMessage(queueItem);
    }

    private async Task InitializeDatabaseAsync()
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='User' AND xtype='U')
                CREATE TABLE [dbo].[User] (
                    [ID]            INT IDENTITY(1,1)   NOT NULL,
                    [FirstName]     NVARCHAR (255)      NOT NULL,
                    [LastName]      NVARCHAR (255)      NOT NULL,
                    [Email]         NVARCHAR(100)       NOT NULL,
                    [ExternalID]    NVARCHAR(255)       NOT NULL,
                    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC)
                )";
        await connection.ExecuteAsync(sql);
    }


    private async Task ProcessQueueMessage(string message)
    {
        var createUserEventMessage = JsonSerializer.Deserialize<CreateUserEventMessage>(message);
        if (createUserEventMessage == null)
        {
            throw new InvalidDataException($"Invalid message: {message}");
        }

        var newUser = User.CreateFake(createUserEventMessage.ExternalUserID);
        await SaveUserToBlobAsync(newUser);
        await SaveUserToTableStorageAsync(newUser);
    }

    private async Task SaveUserToTableStorageAsync(User user)
    {
        try
        {
            var tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var tableClient = tableServiceClient.GetTableClient("Az204");
            await tableClient.CreateIfNotExistsAsync();

            if (await UserExistsAsync(tableClient, user.RowKey))
            {
                _logger.LogWarning("User with RowKey {RowKey} already exists.", user.RowKey);

                return;
            }

            await tableClient.AddEntityAsync(new TableEntity
            {
                PartitionKey = PartitionKey,
                RowKey = user.RowKey,
                ["Data"] = JsonSerializer.Serialize(user) // Store user as JSON in a dynamic property
            });
            _logger.LogWarning("User with RowKey:{RowKey} created to table storage.", user.RowKey);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogError(ex, "Could not find user table entity");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save user to table storage");

            throw;
        }
    }

    private async Task SaveUserToBlobAsync(User user)
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var container = blobServiceClient.GetBlobContainerClient("az-204-containers");
            await container.CreateIfNotExistsAsync();

            var blobClient = container.GetAppendBlobClient("users.json");
            await blobClient.CreateIfNotExistsAsync();

            using var memoryStream = new MemoryStream();
            var contentBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user));
            memoryStream.Write(contentBytes, 0, contentBytes.Length);
            memoryStream.Position = 0;

            await blobClient.AppendBlockAsync(memoryStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to append user data to blob");

            throw;
        }
    }

    private static async Task<bool> UserExistsAsync(TableClient tableClient, string rowKey)
    {
        try
        {
            await tableClient.GetEntityAsync<TableEntity>(PartitionKey, rowKey);

            return true;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }
}