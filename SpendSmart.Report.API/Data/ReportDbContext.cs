using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Report.API.Data
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
    }

    public class Expense
    {
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
    }

    public class Income
    {
        public int IncomeId { get; set; }
        public int UserId { get; set; }
        public string Source { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Actor { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Data { get; set; }
    }
}
