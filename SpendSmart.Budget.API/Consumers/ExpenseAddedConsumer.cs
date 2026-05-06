using MassTransit;
using SpendSmart.Shared.Events;
using SpendSmart.Budget.API.Services;

namespace SpendSmart.Budget.API.Consumers
{
    public class ExpenseAddedConsumer : IConsumer<ExpenseAddedEvent>
    {
        private readonly IBudgetService _budgetService;
        private readonly ILogger<ExpenseAddedConsumer> _logger;

        public ExpenseAddedConsumer(IBudgetService budgetService, ILogger<ExpenseAddedConsumer> logger)
        {
            _budgetService = budgetService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ExpenseAddedEvent> context)
        {
            var data = context.Message;
            _logger.LogInformation("Processing ExpenseAddedEvent for User {UserId}, Category {CategoryId}, Amount {Amount}", 
                data.UserId, data.CategoryId, data.Amount);

            try
            {
                await _budgetService.CheckBudgetOnExpenseAsync(data.UserId, data.CategoryId, data.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process budget check for expense {ExpenseId}", data.ExpenseId);
            }
        }
    }
}
