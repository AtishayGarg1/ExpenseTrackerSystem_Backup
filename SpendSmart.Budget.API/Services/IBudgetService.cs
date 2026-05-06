using SpendSmart.Budget.API.Entities;

namespace SpendSmart.Budget.API.Services
{
    public interface IBudgetService
    {
        Task<BudgetEntity?> CreateBudgetAsync(int userId, int? categoryId, string name, decimal limitAmount, string currency, string period, DateTime startDate, DateTime endDate);
        Task<BudgetEntity?> GetBudgetByIdAsync(int budgetId, int userId);
        Task<List<BudgetEntity>> GetActiveBudgetsAsync(int userId);
        Task<BudgetEntity?> GetBudgetByCategoryAsync(int userId, int categoryId);
        Task<List<BudgetEntity>> GetOverBudgetAlertsAsync(int userId);
        Task<bool> DeleteBudgetAsync(int budgetId, int userId);
        Task<bool> UpdateBudgetAsync(int budgetId, int userId, int? categoryId, string name, decimal limitAmount, string period, DateTime startDate, DateTime endDate);
        
        Task CheckBudgetOnExpenseAsync(int userId, int categoryId, decimal amount);
    }
}
