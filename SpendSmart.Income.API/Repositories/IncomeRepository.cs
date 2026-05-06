using Microsoft.EntityFrameworkCore;
using SpendSmart.Income.API.Data;
using SpendSmart.Income.API.Entities;

namespace SpendSmart.Income.API.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly IncomeDbContext _context;

        public IncomeRepository(IncomeDbContext context)
        {
            _context = context;
        }

        public async Task<Entities.Income?> FindByIncomeIdAsync(int incomeId)
        {
            return await _context.Incomes.FindAsync(incomeId);
        }

        public async Task<List<Entities.Income>> FindByUserIdAsync(int userId)
        {
            return await _context.Incomes.Where(i => i.UserId == userId).ToListAsync();
        }

        public async Task<List<Entities.Income>> FindBySourceAsync(int userId, string source)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId && i.Source == source)
                .ToListAsync();
        }

        public async Task<List<Entities.Income>> FindByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId && i.Date >= startDate && i.Date <= endDate)
                .ToListAsync();
        }

        public async Task<List<Entities.Income>> FindRecurringAsync(int userId)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId && i.IsRecurring)
                .ToListAsync();
        }

        public async Task<decimal> SumByUserIdAsync(int userId)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId)
                .SumAsync(i => i.Amount);
        }

        public async Task<decimal> SumByMonthAsync(int userId, int month, int year)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId && i.Date.Month == month && i.Date.Year == year)
                .SumAsync(i => i.Amount);
        }

        public async Task<decimal> SumBySourceAsync(int userId, string source)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId && i.Source == source)
                .SumAsync(i => i.Amount);
        }

        public async Task AddIncomeAsync(Entities.Income income)
        {
            await _context.Incomes.AddAsync(income);
        }

        public async Task DeleteIncomeAsync(Entities.Income income)
        {
            _context.Incomes.Remove(income);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
