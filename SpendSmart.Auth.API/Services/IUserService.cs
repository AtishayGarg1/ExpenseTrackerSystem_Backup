using SpendSmart.Auth.API.Entities;

namespace SpendSmart.Auth.API.Services
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(string fullName, string email, string password, string currency);
        Task<string?> LoginAsync(string email, string password);
        Task<string?> AdminLoginAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, string? fullName, string? avatarUrl);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> UpdateCurrencyAsync(int userId, string currency);
        Task<bool> DeactivateAccountAsync(int userId);
        Task<bool> ActivateAccountAsync(int userId);
        Task<bool> DeleteAccountAsync(int userId, string? password = null);
        Task<List<User>> GetActiveUsersAsync();
        Task<List<User>> GetAllUsersAsync();
    }
}
