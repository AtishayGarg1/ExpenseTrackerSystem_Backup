using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendSmart.Notification.API.Data;

namespace SpendSmart.Notification.API.Controllers
{
    [ApiController]
    [Route("api/notifications/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly NotificationDbContext _context;

        public AdminController(NotificationDbContext context)
        {
            _context = context;
        }

        public record BroadcastRequest(string Title, string Message, string Type);

        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastRequest request)
        {
            var activeUsers = await _context.Users.Where(u => u.IsActive).ToListAsync();
            
            var notifications = activeUsers.Select(u => new Data.Notification
            {
                UserId = u.UserId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                SentAt = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            return Ok(new { Count = notifications.Count });
        }
    }
}
