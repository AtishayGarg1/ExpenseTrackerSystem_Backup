using SpendSmart.Expense.API.Entities;

namespace SpendSmart.Expense.API.Repositories
{
    public interface IExpenseRepository
    {
        Task<Entities.Expense?> FindByExpenseIdAsync(int expenseId);
        Task<List<Entities.Expense>> FindByUserIdAsync(int userId);
        Task<List<Entities.Expense>> FindByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<Entities.Expense>> FindByUserIdAndCategoryAsync(int userId, int categoryId);
        Task<List<Entities.Expense>> FindByPaymentModeAsync(int userId, string paymentMode);
        Task<List<Entities.Expense>> FindRecurringAsync(int userId);
        Task<decimal> SumByUserIdAsync(int userId);
        Task<decimal> SumByMonthAsync(int userId, int month, int year);
        Task<decimal> SumByCategoryAsync(int userId, int categoryId);
        Task<List<Entities.Expense>> SearchByKeywordAsync(int userId, string keyword);
        Task AddExpenseAsync(Entities.Expense expense);
        Task DeleteExpenseAsync(Entities.Expense expense);
        Task SaveChangesAsync();
    }
}
