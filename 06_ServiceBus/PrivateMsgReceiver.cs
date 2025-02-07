using System;
using System.Threading.Tasks;
using _06_ServiceBus.Config;
using Azure.Messaging.ServiceBus;

namespace _06_ServiceBus;

class PrivateMsgReceiver
{
    public static async Task RunPrivateMsgReceiverAsync(AzureServiceBusConfig config)
    {
        await ReceiveSalesMessagesAsync(config.ConnectionString, config.QueueName);
    }

    private static async Task ReceiveSalesMessagesAsync(string connectionString, string queueName)
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
        Console.WriteLine("======================================================");

        // Create a Service Bus client that will authenticate using a connection string
        await using var client = new ServiceBusClient(connectionString);

        // Create the options to use for configuring the processor
        var processorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        };

        // Create a processor that we can use to process the messages
        await using ServiceBusProcessor processor = client.CreateProcessor(queueName, processorOptions);

        // Configure the message and error handler to use
        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // Start processing
        await processor.StartProcessingAsync();

        Console.Read();

        // Close the processor here
        await processor.CloseAsync();
    }

    // handle received messages
    private static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");

        // complete the message. messages is deleted from the queue. 
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());

        return Task.CompletedTask;
    }
}