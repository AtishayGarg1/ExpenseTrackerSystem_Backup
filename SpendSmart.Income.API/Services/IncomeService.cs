using SpendSmart.Income.API.Entities;
using SpendSmart.Income.API.Repositories;
using System.Text.Json;

namespace SpendSmart.Income.API.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IncomeService(IIncomeRepository incomeRepository, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _incomeRepository = incomeRepository;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Entities.Income?> AddIncomeAsync(int userId, string source, decimal amount, string currency, string description, DateTime date, bool isRecurring, string? recurrenceType)
        {
            var income = new Entities.Income
            {
                UserId = userId,
                Source = source,
                Amount = amount,
                Currency = currency,
                Description = description,
                Date = date,
                IsRecurring = isRecurring,
                RecurrenceType = recurrenceType
            };

            await _incomeRepository.AddIncomeAsync(income);
            await _incomeRepository.SaveChangesAsync();
            return income;
        }

        // Standard Getters left mostly out for brevity but fully implemented via Repo
        public async Task<Entities.Income?> GetIncomeByIdAsync(int incomeId, int userId)
        {
            var inc = await _incomeRepository.FindByIncomeIdAsync(incomeId);
            return (inc != null && inc.UserId == userId) ? inc : null;
        }
        
        public async Task<List<Entities.Income>> GetIncomesByUserAsync(int userId) => await _incomeRepository.FindByUserIdAsync(userId);
        public async Task<List<Entities.Income>> GetBySourceAsync(int userId, string source) => await _incomeRepository.FindBySourceAsync(userId, source);
        public async Task<List<Entities.Income>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate) => await _incomeRepository.FindByDateRangeAsync(userId, startDate, endDate);
        public async Task<List<Entities.Income>> GetRecurringIncomesAsync(int userId) => await _incomeRepository.FindRecurringAsync(userId);
        public async Task<decimal> GetTotalIncomeAsync(int userId) => await _incomeRepository.SumByUserIdAsync(userId);
        public async Task<decimal> GetTotalForMonthAsync(int userId, int month, int year) => await _incomeRepository.SumByMonthAsync(userId, month, year);
        public async Task<decimal> GetTotalBySourceAsync(int userId, string source) => await _incomeRepository.SumBySourceAsync(userId, source);

        public async Task<decimal> GetNetBalanceAsync(int userId)
        {
            var totalIncome = await GetTotalIncomeAsync(userId);
            decimal totalExpense = 0;

            try 
            {
                // Internal service-to-service call should hit port 5003 directly to avoid YARP re-entry loops
                var client = _httpClientFactory.CreateClient("ExpenseApi");
                
                // Forward the user's JWT token
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                }

                var response = await client.GetAsync($"/api/expenses/total"); 
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var expenseData = JsonSerializer.Deserialize<Dictionary<string, decimal>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (expenseData != null && expenseData.TryGetValue("total", out var total))
                    {
                        totalExpense = total;
                    }
                }
                else 
                {
                    Console.WriteLine($"[NetBalance] Expense API returned {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NetBalance] Critical failure fetching expenses: {ex.Message}");
                // We default to 0 expenses to allow the service to keep running
            }
            
            return totalIncome - totalExpense;
        }

        public async Task<bool> UpdateIncomeAsync(int incomeId, int userId, string source, decimal amount, string description, DateTime date, bool isRecurring, string? recurrenceType)
        {
            var income = await _incomeRepository.FindByIncomeIdAsync(incomeId);
            if (income == null || income.UserId != userId) return false;

            income.Source = source;
            income.Amount = amount;
            income.Description = description;
            income.Date = date;
            income.IsRecurring = isRecurring;
            income.RecurrenceType = recurrenceType;
            income.UpdatedAt = DateTime.UtcNow;

            await _incomeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteIncomeAsync(int incomeId, int userId)
        {
            var income = await _incomeRepository.FindByIncomeIdAsync(incomeId);
            if (income == null || income.UserId != userId) return false;

            await _incomeRepository.DeleteIncomeAsync(income);
            await _incomeRepository.SaveChangesAsync();
            return true;
        }
    }
}
