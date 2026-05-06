using SpendSmart.Budget.API.Entities;

namespace SpendSmart.Budget.API.Repositories
{
    public interface IBudgetRepository
    {
        Task<BudgetEntity?> FindByBudgetIdAsync(int budgetId);
        Task<List<BudgetEntity>> FindByUserIdAsync(int userId);
        Task<List<BudgetEntity>> FindActiveByUserIdAsync(int userId);
        Task<BudgetEntity?> FindByCategoryIdAsync(int userId, int categoryId);
        Task<BudgetEntity?> FindByGlobalAsync(int userId);
        Task<List<BudgetEntity>> FindOverBudgetAsync(int userId);
        Task AddBudgetAsync(BudgetEntity budget);
        Task UpdateSpentAmountAsync(int budgetId, decimal amount);
        Task DeleteByBudgetIdAsync(int budgetId);
        Task SaveChangesAsync();
    }
}
