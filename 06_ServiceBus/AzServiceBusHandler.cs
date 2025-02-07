using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace _06_ServiceBus
{
    public class AzServiceBusHandler
    {
        private readonly ILogger<AzServiceBusHandler> _logger;

        public AzServiceBusHandler(ILogger<AzServiceBusHandler> logger)
        {
            _logger = logger;
        }

        [Function(nameof(AzServiceBusHandler))]
        public async Task Run(
            [ServiceBusTrigger("myqueue", Connection = "")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
