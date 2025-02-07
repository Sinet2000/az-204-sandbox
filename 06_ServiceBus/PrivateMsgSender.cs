using System;
using System.Threading.Tasks;
using _06_ServiceBus.Config;
using Azure.Messaging.ServiceBus;

namespace _06_ServiceBus;

class PrivateMsgSender
{
    public static async Task RunPrivateMsgSenderAsync(AzureServiceBusConfig config)
    {
        await SendSalesMessageAsync(config.ConnectionString, config.QueueName);
    }

    private static async Task SendSalesMessageAsync(string connectionString, string queueName)
    {
        // Create a Service Bus client here
        await using var client = new ServiceBusClient(connectionString);

        // Create a sender here
        await using var sender = client.CreateSender(queueName);

        try
        {
            // Create and send a message here
            string messageBody = "$10,000 order for bicycle parts from retailer Adventure Works.";
            var message = new ServiceBusMessage(messageBody);
            Console.WriteLine($"Sending message: {messageBody}");
            
            await sender.SendMessageAsync(message);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
        }
    }
}