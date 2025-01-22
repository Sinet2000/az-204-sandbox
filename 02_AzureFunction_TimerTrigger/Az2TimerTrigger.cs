using System;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace _02_AzureFunction_TimerTrigger
{
    public class Az2TimerTrigger(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("MyConnectionString")!;

        [Function(nameof(Az2TimerTrigger))]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(Az2TimerTrigger));
            logger.LogInformation("C# Timer trigger function executed at: {Time}", DateTime.Now);

            // Your database cleanup logic here
            // CleanupDatabase();

            logger.LogInformation("Database cleanup completed successfully.");
            if (myTimer.ScheduleStatus is not null)
            {
                logger.LogInformation("Next timer schedule at: {NextSchedule}", myTimer.ScheduleStatus.Next);
            }
        }

        private void CleanupDatabase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM YourTable WHERE CreatedAt < DATEADD(DAY, -30, GETDATE())";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
