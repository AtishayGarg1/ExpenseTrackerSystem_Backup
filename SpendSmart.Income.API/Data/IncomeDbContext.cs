using Microsoft.EntityFrameworkCore;
using SpendSmart.Income.API.Entities;

namespace SpendSmart.Income.API.Data
{
    public class IncomeDbContext : DbContext
    {
        public IncomeDbContext(DbContextOptions<IncomeDbContext> options) : base(options) { }

        public DbSet<Entities.Income> Incomes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Index for optimizing source-based filters for users
            modelBuilder.Entity<Entities.Income>()
                .HasIndex(i => new { i.UserId, i.Source });
                
            modelBuilder.Entity<Entities.Income>()
                .Property(i => i.Amount)
                .HasColumnType("numeric(18,2)");
        }
    }
}
