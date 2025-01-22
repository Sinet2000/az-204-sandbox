using _03_AzureFunction_QueueTrigger.Data.Entities;
using Dapper;

namespace _03_AzureFunction_QueueTrigger.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using (var connection = await _connectionFactory.CreateConnectionAsync())
        {
            return await connection.QueryAsync<User>("SELECT * FROM Users");
        }
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using (var connection = await _connectionFactory.CreateConnectionAsync())
        {
            return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE UserId = @Id", new { Id = id });
        }
    }

    public async Task<int> CreateUserAsync(User user)
    {
        using (var connection = await _connectionFactory.CreateConnectionAsync())
        {
            var sql = "INSERT INTO Users (FirstName, LastName, Email) VALUES (@FirstName, @LastName, @Email); SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, user);
        }
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        using (var connection = await _connectionFactory.CreateConnectionAsync())
        {
            var sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email WHERE UserId = @UserId";
            var rowsAffected = await connection.ExecuteAsync(sql, user);
            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        using (var connection = await _connectionFactory.CreateConnectionAsync())
        {
            var sql = "DELETE FROM Users WHERE UserId = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}