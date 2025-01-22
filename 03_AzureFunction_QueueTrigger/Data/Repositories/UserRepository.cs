using _03_AzureFunction_QueueTrigger.Data.Config;
using _03_AzureFunction_QueueTrigger.Domain.Entities;
using Dapper;

namespace _03_AzureFunction_QueueTrigger.Data.Repositories;

public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using var connection = await connectionFactory.CreateConnectionAsync();

        return await connection.QueryAsync<User>("SELECT * FROM [User]");
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using (var connection = await connectionFactory.CreateConnectionAsync())
        {
            return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM [User] WHERE UserId = @Id", new { Id = id });
        }
    }

    public async Task<int> CreateUserAsync(User user)
    {
        using (var connection = await connectionFactory.CreateConnectionAsync())
        {
            var sql =
                "INSERT INTO [User] (FirstName, LastName, Email) VALUES (@FirstName, @LastName, @Email); SELECT CAST(SCOPE_IDENTITY() as int)";

            return await connection.ExecuteScalarAsync<int>(sql, user);
        }
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        using (var connection = await connectionFactory.CreateConnectionAsync())
        {
            var sql = "UPDATE [User] SET FirstName = @FirstName, LastName = @LastName, Email = @Email WHERE UserId = @UserId";
            var rowsAffected = await connection.ExecuteAsync(sql, user);

            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        using (var connection = await connectionFactory.CreateConnectionAsync())
        {
            var sql = "DELETE FROM [User] WHERE UserId = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

            return rowsAffected > 0;
        }
    }
}