using System.Data;
using Microsoft.Data.SqlClient;

namespace _03_AzureFunction_QueueTrigger.Data.Config
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("MainDbConnectionString")
                                                    ?? throw new InvalidOperationException("Connection string not found.");

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return connection;
        }
    }

    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}