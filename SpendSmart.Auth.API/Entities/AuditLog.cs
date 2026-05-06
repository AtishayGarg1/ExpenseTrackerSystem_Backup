using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Auth.API.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Actor { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Data { get; set; }
    }
}
