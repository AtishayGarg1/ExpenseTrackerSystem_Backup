using MassTransit;
using SpendSmart.Shared.Events;
using SpendSmart.Expense.API.Repositories;

namespace SpendSmart.Expense.API.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExpenseService(IExpenseRepository expenseRepository, IHttpClientFactory httpClientFactory)
        {
            _expenseRepository = expenseRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Entities.Expense?> AddExpenseAsync(int userId, int categoryId, decimal amount, string currency, string description, DateTime date, string paymentMode, string tags, bool isRecurring)
        {
            var expense = new Entities.Expense
            {
                UserId = userId,
                CategoryId = categoryId,
                Amount = amount,
                Currency = currency,
                Description = description,
                Date = date,
                PaymentMode = paymentMode,
                Tags = tags,
                IsRecurring = isRecurring
            };

            await _expenseRepository.AddExpenseAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            // Notify Budget Service directly via HTTP
            try
            {
                var client = _httpClientFactory.CreateClient();
                await client.PostAsJsonAsync("http://localhost:5005/api/budgets/internal/check", new
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    Amount = amount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to notify Budget service: {ex.Message}");
            }

            return expense;
        }

        public async Task<Entities.Expense?> GetExpenseByIdAsync(int expenseId, int userId)
        {
            var expense = await _expenseRepository.FindByExpenseIdAsync(expenseId);
            return (expense != null && expense.UserId == userId) ? expense : null;
        }

        public async Task<List<Entities.Expense>> GetExpensesByUserAsync(int userId)
        {
            return await _expenseRepository.FindByUserIdAsync(userId);
        }

        public async Task<List<Entities.Expense>> GetByCategoryAsync(int userId, int categoryId)
        {
            return await _expenseRepository.FindByUserIdAndCategoryAsync(userId, categoryId);
        }

        public async Task<List<Entities.Expense>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _expenseRepository.FindByDateRangeAsync(userId, startDate, endDate);
        }

        public async Task<List<Entities.Expense>> GetByPaymentModeAsync(int userId, string paymentMode)
        {
            return await _expenseRepository.FindByPaymentModeAsync(userId, paymentMode);
        }

        public async Task<List<Entities.Expense>> GetRecurringExpensesAsync(int userId)
        {
            return await _expenseRepository.FindRecurringAsync(userId);
        }

        public async Task<List<Entities.Expense>> SearchExpensesAsync(int userId, string keyword)
        {
            return await _expenseRepository.SearchByKeywordAsync(userId, keyword);
        }

        public async Task<decimal> GetTotalByUserAsync(int userId)
        {
            return await _expenseRepository.SumByUserIdAsync(userId);
        }

        public async Task<decimal> GetTotalForMonthAsync(int userId, int month, int year)
        {
            return await _expenseRepository.SumByMonthAsync(userId, month, year);
        }

        public async Task<decimal> GetTotalByCategoryAsync(int userId, int categoryId)
        {
            return await _expenseRepository.SumByCategoryAsync(userId, categoryId);
        }

        public async Task<bool> UpdateExpenseAsync(int expenseId, int userId, int categoryId, decimal amount, string description, DateTime date, string paymentMode, string tags)
        {
            var expense = await _expenseRepository.FindByExpenseIdAsync(expenseId);
            if (expense == null || expense.UserId != userId) return false;

            expense.CategoryId = categoryId;
            expense.Amount = amount;
            expense.Description = description;
            expense.Date = date;
            expense.PaymentMode = paymentMode;
            expense.Tags = tags;
            expense.UpdatedAt = DateTime.UtcNow;

            await _expenseRepository.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> UpdateReceiptUrlAsync(int expenseId, int userId, string receiptUrl)
        {
            var expense = await _expenseRepository.FindByExpenseIdAsync(expenseId);
            if (expense == null || expense.UserId != userId) return false;

            expense.ReceiptUrl = receiptUrl;
            await _expenseRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExpenseAsync(int expenseId, int userId)
        {
            var expense = await _expenseRepository.FindByExpenseIdAsync(expenseId);
            if (expense == null || expense.UserId != userId) return false;

            await _expenseRepository.DeleteExpenseAsync(expense);
            await _expenseRepository.SaveChangesAsync();
            return true;
        }
    }
}
