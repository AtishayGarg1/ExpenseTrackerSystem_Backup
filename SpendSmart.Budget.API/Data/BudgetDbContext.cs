using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Entities;

namespace SpendSmart.Budget.API.Data
{
    public class BudgetDbContext : DbContext
    {
        public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) { }

        public DbSet<BudgetEntity> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Ensures exactly one active budget per category per period for a user
            modelBuilder.Entity<BudgetEntity>()
                .HasIndex(b => new { b.UserId, b.CategoryId })
                .IsUnique()
                .HasFilter("\"IsActive\" = true");
                
            modelBuilder.Entity<BudgetEntity>().Property(b => b.LimitAmount).HasColumnType("numeric(18,2)");
            modelBuilder.Entity<BudgetEntity>().Property(b => b.SpentAmount).HasColumnType("numeric(18,2)");
        }
    }
}
