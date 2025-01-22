using System.Data;

namespace _03_AzureFunction_QueueTrigger.Data
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}