using _03_AzureFunction_QueueTrigger.Data;
using _03_AzureFunction_QueueTrigger.Data.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(_03_AzureFunction_QueueTrigger.Startup))]
namespace _03_AzureFunction_QueueTrigger;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddTransient<IDbConnectionFactory, SqlConnectionFactory>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();
    }
}