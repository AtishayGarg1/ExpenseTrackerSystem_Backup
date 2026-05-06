using SpendSmart.Income.API.Entities;

namespace SpendSmart.Income.API.Repositories
{
    public interface IIncomeRepository
    {
        Task<Entities.Income?> FindByIncomeIdAsync(int incomeId);
        Task<List<Entities.Income>> FindByUserIdAsync(int userId);
        Task<List<Entities.Income>> FindBySourceAsync(int userId, string source);
        Task<List<Entities.Income>> FindByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<Entities.Income>> FindRecurringAsync(int userId);
        Task<decimal> SumByUserIdAsync(int userId);
        Task<decimal> SumByMonthAsync(int userId, int month, int year);
        Task<decimal> SumBySourceAsync(int userId, string source);
        Task AddIncomeAsync(Entities.Income income);
        Task DeleteIncomeAsync(Entities.Income income);
        Task SaveChangesAsync();
    }
}
