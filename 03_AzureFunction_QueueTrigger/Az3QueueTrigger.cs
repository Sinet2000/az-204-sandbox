using System;
using _03_AzureFunction_QueueTrigger.Data;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using Dapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace _03_AzureFunction_QueueTrigger
{
    public class Az3QueueTrigger
    {
        private readonly ILogger _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly TableServiceClient _tableServiceClient;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly string _connectionString;

        public Az3QueueTrigger(
            ILoggerFactory loggerFactory,
            BlobServiceClient blobServiceClient,
            TableServiceClient tableServiceClient,
            IConfiguration configuration,
            IDbConnectionFactory dbConnectionFactory)
        {
            _logger = loggerFactory.CreateLogger<Az3QueueTrigger>();
            _blobServiceClient = blobServiceClient;
            _tableServiceClient = tableServiceClient;
            _connectionString = configuration.GetConnectionString("MyConnectionString")!;
            _dbConnectionFactory = dbConnectionFactory;
        }

        [Function(nameof(Az3QueueTrigger))]
        public async Task Run([QueueTrigger("myqueue-items")] string myQueueItem)
        {
            _logger.LogInformation("C# Queue trigger function processed: {QueueItem}", myQueueItem);

            await InitializeDatabaseAsync();
            // Process the queue message
            await ProcessQueueMessage(myQueueItem);
        }

        private async Task InitializeDatabaseAsync()
        {
            using (var connection = await _dbConnectionFactory.CreateConnectionAsync())
            {
                var sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                CREATE TABLE [dbo].[User] (
                    [ID]            INT IDENTITY(1,1)   NOT NULL,
                    [FirstName]     NVARCHAR (255)      NOT NULL,
                    [LastName]      NVARCHAR (255)      NOT NULL,
                    [Email]         NVARCHAR(100)       NOT NULL,
                    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC)
                )";
                await connection.ExecuteAsync(sql);
            }
        }


        private async Task ProcessQueueMessage(string message)
        {
            // Example: Save message to Blob Storage
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("mycontainer");
            var blobClient = blobContainerClient.GetBlobClient($"{Guid.NewGuid()}.txt");
            await blobClient.UploadAsync(new BinaryData(message));

            // Example: Save message to Table Storage
            var tableClient = _tableServiceClient.GetTableClient("mytable");
            var entity = new TableEntity("PartitionKey", Guid.NewGuid().ToString())
            {
                { "Message", message }
            };

            await tableClient.AddEntityAsync(entity);

            // Example: Delete records older than 30 days using Dapper
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "DELETE FROM Records WHERE CreatedDate < @Date";
                await connection.ExecuteAsync(sql, new { Date = DateTime.Now.AddDays(-30) });
            }
        }
    }
}
