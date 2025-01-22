using _03_AzureFunction_QueueTrigger.Data.Entities;

namespace _03_AzureFunction_QueueTrigger.Data.Repositories;
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<int> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}