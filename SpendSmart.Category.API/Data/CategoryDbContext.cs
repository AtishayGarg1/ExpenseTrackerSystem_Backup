using Microsoft.EntityFrameworkCore;
using SpendSmart.Category.API.Entities;

namespace SpendSmart.Category.API.Data
{
    public class CategoryDbContext : DbContext
    {
        public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options) { }

        public DbSet<Entities.Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Category>()
                .HasIndex(c => new { c.UserId, c.Name });
        }
    }
}
