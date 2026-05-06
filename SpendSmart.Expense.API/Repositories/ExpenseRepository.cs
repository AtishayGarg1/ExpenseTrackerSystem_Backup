using Microsoft.EntityFrameworkCore;
using SpendSmart.Expense.API.Data;
using SpendSmart.Expense.API.Entities;

namespace SpendSmart.Expense.API.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseDbContext _context;

        public ExpenseRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<Entities.Expense?> FindByExpenseIdAsync(int expenseId)
        {
            return await _context.Expenses.FindAsync(expenseId);
        }

        public async Task<List<Entities.Expense>> FindByUserIdAsync(int userId)
        {
            return await _context.Expenses.Where(e => e.UserId == userId).ToListAsync();
        }

        public async Task<List<Entities.Expense>> FindByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();
        }

        public async Task<List<Entities.Expense>> FindByUserIdAndCategoryAsync(int userId, int categoryId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<Entities.Expense>> FindByPaymentModeAsync(int userId, string paymentMode)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.PaymentMode == paymentMode)
                .ToListAsync();
        }
        
        public async Task<List<Entities.Expense>> FindRecurringAsync(int userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.IsRecurring)
                .ToListAsync();
        }

        public async Task<decimal> SumByUserIdAsync(int userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> SumByMonthAsync(int userId, int month, int year)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.Date.Month == month && e.Date.Year == year)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> SumByCategoryAsync(int userId, int categoryId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.CategoryId == categoryId)
                .SumAsync(e => e.Amount);
        }

        public async Task<List<Entities.Expense>> SearchByKeywordAsync(int userId, string keyword)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && EF.Functions.Like(e.Description, $"%{keyword}%"))
                .ToListAsync();
        }

        public async Task AddExpenseAsync(Entities.Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
        }

        public async Task DeleteExpenseAsync(Entities.Expense expense)
        {
            _context.Expenses.Remove(expense);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
