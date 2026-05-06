using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Notification.API.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public class Notification
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? RelatedId { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

    public class User
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
