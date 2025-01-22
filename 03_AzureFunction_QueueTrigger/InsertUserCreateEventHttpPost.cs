using System.Net;
using System.Text;
using System.Text.Json;
using _03_AzureFunction_QueueTrigger.Messaging;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;

namespace _03_AzureFunction_QueueTrigger
{
    public class CreateUserHttp
    {
        [Function(nameof(CreateUserHttp))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")] HttpRequestData req, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger(nameof(CreateUserHttp));
            logger.LogInformation("Creating an event to create user");

            try
            {
                var userExternalID = Guid.NewGuid().ToString();

                var message = JsonSerializer.Serialize(new CreateUserEventMessage(nameof(ServiceEvents.CreateUser), userExternalID));
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var queueClient = new QueueClient(connectionString, "az-204-queue-items");

                var queueCreateResponse = await queueClient.CreateIfNotExistsAsync();
                if (queueCreateResponse?.IsError == true)
                {
                    logger.LogError("Failed to create the queue. Error: {Error}", queueCreateResponse);

                    var errResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await errResponse.WriteStringAsync("Bad request: Failed to create the queue.");

                    return errResponse;
                }

                // fIXES the warning: Encoding.UTF8.GetBytes(message)
                await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
                
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Created user create event for UserExternalID:{userExternalID}");

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create the user create event & save to the queue");

                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync(ex.Message);

                return response;
            }
        }
    }
}