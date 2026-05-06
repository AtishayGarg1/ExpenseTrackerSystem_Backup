using SpendSmart.Income.API.Entities;

namespace SpendSmart.Income.API.Services
{
    public interface IIncomeService
    {
        Task<Entities.Income?> AddIncomeAsync(int userId, string source, decimal amount, string currency, string description, DateTime date, bool isRecurring, string? recurrenceType);
        Task<Entities.Income?> GetIncomeByIdAsync(int incomeId, int userId);
        Task<List<Entities.Income>> GetIncomesByUserAsync(int userId);
        Task<List<Entities.Income>> GetBySourceAsync(int userId, string source);
        Task<List<Entities.Income>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<Entities.Income>> GetRecurringIncomesAsync(int userId);
        Task<decimal> GetTotalIncomeAsync(int userId);
        Task<decimal> GetTotalForMonthAsync(int userId, int month, int year);
        Task<decimal> GetTotalBySourceAsync(int userId, string source);
        Task<decimal> GetNetBalanceAsync(int userId); // Needs cross-service call to Expense
        Task<bool> UpdateIncomeAsync(int incomeId, int userId, string source, decimal amount, string description, DateTime date, bool isRecurring, string? recurrenceType);
        Task<bool> DeleteIncomeAsync(int incomeId, int userId);
    }
}
