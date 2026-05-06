using SpendSmart.Auth.API.Entities;

namespace SpendSmart.Auth.API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByUserIdAsync(int userId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<List<User>> FindAllActiveAsync();
        Task<List<User>> FindAllAsync();
        Task DeleteUserAsync(User user);
        Task AddUserAsync(User user);
        Task UpdateLastLoginAsync(int userId, DateTime loginTime);
        Task UpdateCurrencyAsync(int userId, string currency);
        Task SaveChangesAsync();
    }
}
