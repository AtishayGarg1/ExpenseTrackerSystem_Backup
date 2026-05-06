using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Data;
using SpendSmart.Budget.API.Entities;

namespace SpendSmart.Budget.API.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly BudgetDbContext _context;

        public BudgetRepository(BudgetDbContext context)
        {
            _context = context;
        }

        public async Task<BudgetEntity?> FindByBudgetIdAsync(int budgetId)
        {
            return await _context.Budgets.FindAsync(budgetId);
        }

        public async Task<List<BudgetEntity>> FindByUserIdAsync(int userId)
        {
            return await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<List<BudgetEntity>> FindActiveByUserIdAsync(int userId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive && b.StartDate <= DateTime.UtcNow && b.EndDate >= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<BudgetEntity?> FindByCategoryIdAsync(int userId, int categoryId)
        {
            return await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId && b.IsActive);
        }

        public async Task<BudgetEntity?> FindByGlobalAsync(int userId)
        {
            return await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == null && b.IsActive);
        }

        public async Task<List<BudgetEntity>> FindOverBudgetAsync(int userId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive && b.SpentAmount >= b.LimitAmount)
                .ToListAsync();
        }

        public async Task AddBudgetAsync(BudgetEntity budget)
        {
            await _context.Budgets.AddAsync(budget);
        }

        public async Task UpdateSpentAmountAsync(int budgetId, decimal amount)
        {
            // ExecuteUpdateAsync prevents race conditions when concurrently adding expenses
            await _context.Budgets
                .Where(b => b.BudgetId == budgetId)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.SpentAmount, b => b.SpentAmount + amount));
        }

        public async Task DeleteByBudgetIdAsync(int budgetId)
        {
            await _context.Budgets
                .Where(b => b.BudgetId == budgetId)
                .ExecuteDeleteAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
