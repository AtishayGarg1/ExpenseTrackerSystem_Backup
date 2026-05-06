using Microsoft.EntityFrameworkCore;
using SpendSmart.Category.API.Data;
using SpendSmart.Category.API.Entities;

namespace SpendSmart.Category.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDbContext _context;

        public CategoryRepository(CategoryDbContext context)
        {
            _context = context;
        }

        public async Task<Entities.Category?> FindByCategoryIdAsync(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<List<Entities.Category>> FindByUserIdAsync(int userId)
        {
            return await _context.Categories
                .Where(c => (c.UserId == userId || c.UserId == null || c.IsDefault == true) && c.IsActive)
                .ToListAsync();
        }

        public async Task<List<Entities.Category>> FindDefaultCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => (c.IsDefault == true || c.UserId == null) && c.IsActive)
                .ToListAsync();
        }

        public async Task<List<Entities.Category>> FindByTypeAsync(int userId, string type)
        {
            return await _context.Categories
                .Where(c => (c.UserId == userId || c.UserId == null || c.IsDefault == true) 
                            && c.Type == type 
                            && c.IsActive)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(int? userId, string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.UserId == userId && c.Name == name && c.IsActive);
        }

        public async Task<List<Entities.Category>> FindAllForUserAsync(int userId)
        {
            return await _context.Categories
                .Where(c => (c.UserId == userId || c.UserId == null || c.IsDefault == true) && c.IsActive)
                .ToListAsync();
        }

        public async Task AddCategoryAsync(Entities.Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task DeactivateCategoryAsync(int categoryId)
        {
            await _context.Categories
                .Where(c => c.CategoryId == categoryId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsActive, false));
        }

        public async Task DeleteCategoryAsync(Entities.Category category)
        {
            _context.Categories.Remove(category);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
