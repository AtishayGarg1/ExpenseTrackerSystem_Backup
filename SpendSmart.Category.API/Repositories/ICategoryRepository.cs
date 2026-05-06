using SpendSmart.Category.API.Entities;

namespace SpendSmart.Category.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<Entities.Category?> FindByCategoryIdAsync(int categoryId);
        Task<List<Entities.Category>> FindByUserIdAsync(int userId);
        Task<List<Entities.Category>> FindDefaultCategoriesAsync();
        Task<List<Entities.Category>> FindByTypeAsync(int userId, string type);
        Task<bool> ExistsByNameAsync(int? userId, string name);
        Task<List<Entities.Category>> FindAllForUserAsync(int userId); // UNION of defaults + user categories
        Task AddCategoryAsync(Entities.Category category);
        Task DeactivateCategoryAsync(int categoryId);
        Task DeleteCategoryAsync(Entities.Category category);
        Task SaveChangesAsync();
    }
}
