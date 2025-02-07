using System;
using _06_ServiceBus.Config;
using Microsoft.Extensions.Configuration;

namespace _06_ServiceBus;

class Program
{
    const string ServiceBusConnectionString = "<CONNECTION STRING>";
    const string TopicName = "salesperformancemessages";
    const string SubscriptionName = "Americas";

    static void Main(string[] args)
    {
        var config = LoadConfiguration();
        var serviceBusConfig = new AzureServiceBusConfig();
        config.GetSection("AzureServiceBus").Bind(serviceBusConfig);

        PrivateMsgSender.RunPrivateMsgSenderAsync(serviceBusConfig).GetAwaiter().GetResult();
        PrivateMsgReceiver.RunPrivateMsgReceiverAsync(serviceBusConfig).GetAwaiter().GetResult();
    }

    private static IConfiguration LoadConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return configBuilder;
    }
}