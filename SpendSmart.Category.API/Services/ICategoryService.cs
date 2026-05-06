using SpendSmart.Category.API.Entities;

namespace SpendSmart.Category.API.Services
{
    public interface ICategoryService
    {
        Task<Entities.Category?> CreateCategoryAsync(Entities.Category category);
        Task<Entities.Category?> GetCategoryByIdAsync(int categoryId);
        Task<List<Entities.Category>> GetCategoriesByUserAsync(int userId);
        Task<List<Entities.Category>> GetDefaultCategoriesAsync();
        Task<List<Entities.Category>> GetAllForUserAsync(int userId);
        Task<List<Entities.Category>> GetByTypeAsync(int userId, string type);
        Task<bool> UpdateCategoryAsync(int categoryId, int userId, string name, string icon, string color);
        Task<bool> DeactivateCategoryAsync(int categoryId, int userId);
        Task<bool> DeleteCategoryAsync(int categoryId, int userId);
        Task SeedSystemDefaultsAsync();
    }
}
