using Microsoft.EntityFrameworkCore;
using SpendSmart.Category.API.Entities;
using SpendSmart.Category.API.Repositories;

namespace SpendSmart.Category.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly Data.CategoryDbContext _context;

        public CategoryService(ICategoryRepository categoryRepository, Data.CategoryDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public async Task<Entities.Category?> CreateCategoryAsync(Entities.Category category)
        {
            if (await _categoryRepository.ExistsByNameAsync(category.UserId, category.Name))
                return null;

            await _categoryRepository.AddCategoryAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }

        public async Task<Entities.Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _categoryRepository.FindByCategoryIdAsync(categoryId);
        }

        public async Task<List<Entities.Category>> GetCategoriesByUserAsync(int userId)
        {
            return await _categoryRepository.FindByUserIdAsync(userId);
        }

        public async Task<List<Entities.Category>> GetDefaultCategoriesAsync()
        {
            return await _categoryRepository.FindDefaultCategoriesAsync();
        }

        public async Task<List<Entities.Category>> GetAllForUserAsync(int userId)
        {
            return await _categoryRepository.FindAllForUserAsync(userId);
        }

        public async Task<List<Entities.Category>> GetByTypeAsync(int userId, string type)
        {
            return await _categoryRepository.FindByTypeAsync(userId, type);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, int userId, string name, string icon, string color)
        {
            var category = await _categoryRepository.FindByCategoryIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                return false;

            category.Name = name;
            category.Icon = icon;
            category.Color = color;

            await _categoryRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateCategoryAsync(int categoryId, int userId)
        {
            var category = await _categoryRepository.FindByCategoryIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                return false;

            await _categoryRepository.DeactivateCategoryAsync(categoryId);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId, int userId)
        {
            var category = await _categoryRepository.FindByCategoryIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                return false;

            await _categoryRepository.DeleteCategoryAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return true;
        }

        public async Task SeedSystemDefaultsAsync()
        {
            var defaults = new List<Entities.Category>
            {
                new() { Name = "Food", Icon = "🍔", Color = "#FF6B6B", Type = "EXPENSE", IsDefault = true, UserId = null },
                new() { Name = "Transport", Icon = "🚗", Color = "#4D96FF", Type = "EXPENSE", IsDefault = true, UserId = null },
                new() { Name = "Entertainment", Icon = "🎬", Color = "#9D72FF", Type = "EXPENSE", IsDefault = true, UserId = null },
                new() { Name = "Health", Icon = "🏥", Color = "#FF8787", Type = "EXPENSE", IsDefault = true, UserId = null },
                new() { Name = "Shopping", Icon = "🛍️", Color = "#FFD93D", Type = "EXPENSE", IsDefault = true, UserId = null },
                new() { Name = "Bills", Icon = "📄", Color = "#6BCB77", Type = "EXPENSE", IsDefault = true, UserId = null },
                new() { Name = "Education", Icon = "🎓", Color = "#4F709C", Type = "EXPENSE", IsDefault = true, UserId = null },
                new() { Name = "Salary", Icon = "💰", Color = "#2ecc71", Type = "INCOME", IsDefault = true, UserId = null },
                new() { Name = "Freelance", Icon = "👨‍💻", Color = "#3498db", Type = "INCOME", IsDefault = true, UserId = null },
                new() { Name = "Investment", Icon = "📈", Color = "#f1c40f", Type = "INCOME", IsDefault = true, UserId = null }
            };

            foreach (var cat in defaults)
            {
                if (!await _context.Categories.AnyAsync(c => c.Name == cat.Name && c.UserId == null))
                {
                    _context.Categories.Add(cat);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
