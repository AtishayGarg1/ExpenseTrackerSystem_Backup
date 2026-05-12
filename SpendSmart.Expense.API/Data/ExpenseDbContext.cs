using Microsoft.EntityFrameworkCore;
using SpendSmart.Expense.API.Entities;

namespace SpendSmart.Expense.API.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) { }

        public DbSet<Entities.Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Composite index for common date range queries per user
            modelBuilder.Entity<Entities.Expense>()
                .HasIndex(e => new { e.UserId, e.Date });
                
            modelBuilder.Entity<Entities.Expense>()
                .Property(e => e.Amount)
                .HasColumnType("numeric(18,2)");
        }
    }
}
