using SpendSmart.Expense.API.Entities;

namespace SpendSmart.Expense.API.Services
{
    public interface IExpenseService
    {
        Task<Entities.Expense?> AddExpenseAsync(int userId, int categoryId, decimal amount, string currency, string description, DateTime date, string paymentMode, string tags, bool isRecurring);
        Task<Entities.Expense?> GetExpenseByIdAsync(int expenseId, int userId);
        Task<List<Entities.Expense>> GetExpensesByUserAsync(int userId);
        Task<List<Entities.Expense>> GetByCategoryAsync(int userId, int categoryId);
        Task<List<Entities.Expense>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<Entities.Expense>> GetByPaymentModeAsync(int userId, string paymentMode);
        Task<List<Entities.Expense>> GetRecurringExpensesAsync(int userId);
        Task<List<Entities.Expense>> SearchExpensesAsync(int userId, string keyword);
        Task<decimal> GetTotalByUserAsync(int userId);
        Task<decimal> GetTotalForMonthAsync(int userId, int month, int year);
        Task<decimal> GetTotalByCategoryAsync(int userId, int categoryId);
        Task<bool> UpdateExpenseAsync(int expenseId, int userId, int categoryId, decimal amount, string description, DateTime date, string paymentMode, string tags);
        Task<bool> UpdateReceiptUrlAsync(int expenseId, int userId, string receiptUrl);
        Task<bool> DeleteExpenseAsync(int expenseId, int userId);
    }
}
