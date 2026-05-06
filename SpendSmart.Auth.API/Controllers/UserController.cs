using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Auth.API.Services;
using System.Security.Claims;

namespace SpendSmart.Auth.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SpendSmart.Auth.API.Data.AuthDbContext _context;

        public UserController(IUserService userService, SpendSmart.Auth.API.Data.AuthDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public record RegisterRequest(string FullName, string Email, string Password, string Currency);
        public record LoginRequest(string Email, string Password);
        public record UpdateProfileRequest(string? FullName, string? AvatarUrl);
        public record ChangePasswordRequest(string OldPassword, string NewPassword);
        public record UpdateCurrencyRequest(string Currency);
        public record DeleteAccountRequest(string Password);

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request.FullName, request.Email, request.Password, request.Currency);
            if (user == null) return BadRequest(new { Message = "Email already in use." });
            return Ok(new { user.UserId, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _userService.LoginAsync(request.Email, request.Password);
            if (token == null) return Unauthorized(new { Message = "Invalid credentials." });
            return Ok(new { Token = token });
        }

        [HttpPost("login/admin")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
        {
            var token = await _userService.AdminLoginAsync(request.Email, request.Password);
            if (token == null) return Unauthorized(new { Message = "Invalid admin credentials or insufficient permissions." });
            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(new { user.UserId, user.FullName, user.Email, user.Currency, user.AvatarUrl });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _userService.UpdateProfileAsync(userId, request.FullName, request.AvatarUrl);
            return success ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _userService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);
            return success ? Ok() : BadRequest(new { Message = "Incorrect old password." });
        }

        [Authorize]
        [HttpPut("currency")]
        public async Task<IActionResult> UpdateCurrency([FromBody] UpdateCurrencyRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _userService.UpdateCurrencyAsync(userId, request.Currency);
            return success ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpDelete("deactivate")]
        public async Task<IActionResult> Deactivate()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _userService.DeactivateAccountAsync(userId);
            return success ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpPost("me/delete")]
        public async Task<IActionResult> DeleteMe([FromBody] DeleteAccountRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _userService.DeleteAccountAsync(userId, request.Password);
            return success ? Ok() : BadRequest(new { Message = "Incorrect password." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var userToDelete = await _userService.GetUserByIdAsync(userId);
            var success = await _userService.DeleteAccountAsync(userId);
            if (success)
            {
                await LogAuditAction("Delete User", $"Deleted user {userToDelete?.Email} (ID: {userId})");
            }
            return success ? Ok() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/suspend")]
        public async Task<IActionResult> SuspendUser(int userId)
        {
            var userToSuspend = await _userService.GetUserByIdAsync(userId);
            var success = await _userService.DeactivateAccountAsync(userId);
            if (success)
            {
                await LogAuditAction("Suspend User", $"Suspended user {userToSuspend?.Email} (ID: {userId})");
            }
            return success ? Ok() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/unsuspend")]
        public async Task<IActionResult> UnsuspendUser(int userId)
        {
            var userToUnsuspend = await _userService.GetUserByIdAsync(userId);
            var success = await _userService.ActivateAccountAsync(userId);
            if (success)
            {
                await LogAuditAction("Unsuspend User", $"Unsuspended user {userToUnsuspend?.Email} (ID: {userId})");
            }
            return success ? Ok() : NotFound();
        }

        private async Task LogAuditAction(string action, string data)
        {
            var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Admin";
            _context.AuditLogs.Add(new Entities.AuditLog
            {
                Action = action,
                Actor = adminEmail,
                Timestamp = DateTime.UtcNow,
                Data = data
            });
            await _context.SaveChangesAsync();
        }
    }
}
