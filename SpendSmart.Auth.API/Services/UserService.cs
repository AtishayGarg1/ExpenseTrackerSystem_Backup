using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SpendSmart.Auth.API.Entities;
using SpendSmart.Auth.API.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using MassTransit;
using SpendSmart.Shared.Events;

namespace SpendSmart.Auth.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<User?> RegisterAsync(string fullName, string email, string password, string currency)
        {
            if (await _userRepository.ExistsByEmailAsync(email))
                return null;

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Currency = currency,
                Role = "User" 
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            return await GenerateTokenAsync(email, password, null);
        }

        public async Task<string?> AdminLoginAsync(string email, string password)
        {
            return await GenerateTokenAsync(email, password, "Admin");
        }

        private async Task<string?> GenerateTokenAsync(string email, string password, string? requiredRole)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null || !user.IsActive) return null;

            if (requiredRole != null && user.Role != requiredRole) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed) return null;

            await _userRepository.UpdateLastLoginAsync(user.UserId, DateTime.UtcNow);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "SuperSecretKeyForSpendSmart123456789!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.FindByUserIdAsync(userId);
        }

        public async Task<bool> UpdateProfileAsync(int userId, string? fullName, string? avatarUrl)
        {
            var user = await _userRepository.FindByUserIdAsync(userId);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(fullName)) user.FullName = fullName;
            if (!string.IsNullOrWhiteSpace(avatarUrl)) user.AvatarUrl = avatarUrl;

            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.FindByUserIdAsync(userId);
            if (user == null) return false;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
            if (result == PasswordVerificationResult.Failed) return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCurrencyAsync(int userId, string currency)
        {
            await _userRepository.UpdateCurrencyAsync(userId, currency);
            return true;
        }

        public async Task<bool> DeactivateAccountAsync(int userId)
        {
            var user = await _userRepository.FindByUserIdAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAccountAsync(int userId)
        {
            var user = await _userRepository.FindByUserIdAsync(userId);
            if (user == null) return false;

            user.IsActive = true;
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int userId, string? password = null)
        {
            var user = await _userRepository.FindByUserIdAsync(userId);
            if (user == null) return false;

            // If a password is provided, verify it (used for "Delete Me")
            if (password != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (result == PasswordVerificationResult.Failed) return false;
            }

            await _userRepository.DeleteUserAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _userRepository.FindAllActiveAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.FindAllAsync();
        }
    }
}
