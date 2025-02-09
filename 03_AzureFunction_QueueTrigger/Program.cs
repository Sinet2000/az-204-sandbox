using _03_AzureFunction_QueueTrigger.Data;
using _03_AzureFunction_QueueTrigger.Data.Config;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();