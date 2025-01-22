// using _03_AzureFunction_QueueTrigger.Data;
// using _03_AzureFunction_QueueTrigger.Data.Repositories;
// using Azure.Identity;
// using Microsoft.Azure.Functions.Extensions.DependencyInjection;
// using Microsoft.Extensions.Azure;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
//
// [assembly: FunctionsStartup(typeof(_03_AzureFunction_QueueTrigger.Startup))]
//
// namespace _03_AzureFunction_QueueTrigger;
//
// public class Startup : FunctionsStartup
// {
//     public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
//     {
//         // Useless, don;t do like that, it is for custom, like appsettings I suppose
//         // var context = builder.GetContext();
//         // builder.ConfigurationBuilder
//         //     .SetBasePath(context.ApplicationRootPath)
//         //     .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
//         //     .AddEnvironmentVariables();
//     }
//
//     public override void Configure(IFunctionsHostBuilder builder)
//     {
//
//         builder.Services.AddAzureClients(builder =>
//         {
//             var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
//
//             // builder.AddBlobServiceClient(storageConnectionString);
//             // builder.AddTableServiceClient(storageConnectionString);
//             // builder.AddQueueServiceClient(storageConnectionString);
//
//             // // Use the environment credential by default
//             // builder.UseCredential(new EnvironmentCredential());
//             //
//             // // Set up any default settings
//             // builder.ConfigureDefaults(Configuration.GetSection("AzureWebJobsStorage"));
//         });
//     }
// }