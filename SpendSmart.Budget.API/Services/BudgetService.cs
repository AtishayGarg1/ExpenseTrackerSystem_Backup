using SpendSmart.Budget.API.Entities;
using SpendSmart.Budget.API.Repositories;
using MassTransit;
using SpendSmart.Shared.Events;

namespace SpendSmart.Budget.API.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public BudgetService(IBudgetRepository budgetRepository, IPublishEndpoint publishEndpoint)
        {
            _budgetRepository = budgetRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<BudgetEntity?> CreateBudgetAsync(int userId, int? categoryId, string name, decimal limitAmount, string currency, string period, DateTime startDate, DateTime endDate)
        {
            var budget = new BudgetEntity
            {
                UserId = userId,
                CategoryId = categoryId,
                Name = name,
                LimitAmount = limitAmount,
                Currency = currency,
                Period = period,
                StartDate = startDate,
                EndDate = endDate
            };

            await _budgetRepository.AddBudgetAsync(budget);
            await _budgetRepository.SaveChangesAsync();
            return budget;
        }

        public async Task<BudgetEntity?> GetBudgetByIdAsync(int budgetId, int userId)
        {
            var budget = await _budgetRepository.FindByBudgetIdAsync(budgetId);
            return (budget != null && budget.UserId == userId) ? budget : null;
        }

        public async Task<List<BudgetEntity>> GetActiveBudgetsAsync(int userId)
        {
            return await _budgetRepository.FindActiveByUserIdAsync(userId);
        }

        public async Task<BudgetEntity?> GetBudgetByCategoryAsync(int userId, int categoryId)
        {
            return await _budgetRepository.FindByCategoryIdAsync(userId, categoryId);
        }

        public async Task<List<BudgetEntity>> GetOverBudgetAlertsAsync(int userId)
        {
            return await _budgetRepository.FindOverBudgetAsync(userId);
        }

        public async Task<bool> DeleteBudgetAsync(int budgetId, int userId)
        {
            var budget = await _budgetRepository.FindByBudgetIdAsync(budgetId);
            if (budget == null || budget.UserId != userId) return false;

            await _budgetRepository.DeleteByBudgetIdAsync(budgetId);
            return true;
        }

        public async Task<bool> UpdateBudgetAsync(int budgetId, int userId, int? categoryId, string name, decimal limitAmount, string period, DateTime startDate, DateTime endDate)
        {
            var budget = await _budgetRepository.FindByBudgetIdAsync(budgetId);
            if (budget == null || budget.UserId != userId) return false;

            budget.CategoryId = categoryId;
            budget.Name = name;
            budget.LimitAmount = limitAmount;
            budget.Period = period;
            budget.StartDate = startDate;
            budget.EndDate = endDate;
            budget.UpdatedAt = DateTime.UtcNow;

            await _budgetRepository.SaveChangesAsync();
            return true;
        }

        public async Task CheckBudgetOnExpenseAsync(int userId, int categoryId, decimal amount)
        {
            // 1. Get specific category budget
            var catBudget = await _budgetRepository.FindByCategoryIdAsync(userId, categoryId);
            if (catBudget != null)
            {
                await _budgetRepository.UpdateSpentAmountAsync(catBudget.BudgetId, amount);
                await CheckAlertsAsync(catBudget.BudgetId);
            }

            // 2. Get overall global budget (categoryId = null)
            var globalBudget = await _budgetRepository.FindByGlobalAsync(userId); 
            if (globalBudget != null)
            {
                await _budgetRepository.UpdateSpentAmountAsync(globalBudget.BudgetId, amount);
                await CheckAlertsAsync(globalBudget.BudgetId);
            }
        }

        private async Task CheckAlertsAsync(int budgetId)
        {
            var budget = await _budgetRepository.FindByBudgetIdAsync(budgetId);
            if (budget == null || budget.LimitAmount == 0) return;
            
            var percentage = (budget.SpentAmount / budget.LimitAmount) * 100;
            
            if (percentage >= 100 || percentage >= 80)
            {
                string type = percentage >= 100 ? "LIMIT_REACHED" : "WARNING";
                
                await _publishEndpoint.Publish<BudgetThresholdReachedEvent>(new
                {
                    budget.UserId,
                    budget.BudgetId,
                    BudgetName = budget.Name,
                    Type = type,
                    Percentage = percentage,
                    budget.SpentAmount,
                    budget.LimitAmount
                });

                Console.WriteLine($"[{type}] Published alert for User {budget.UserId} on budget {budget.Name} ({percentage}%)");
            }
        }
    }
}
