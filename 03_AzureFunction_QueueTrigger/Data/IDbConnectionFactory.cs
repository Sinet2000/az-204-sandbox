using System.Data;

namespace _03_AzureFunction_QueueTrigger
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}