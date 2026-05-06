using MassTransit;
using SpendSmart.Budget.API.Services;
using SpendSmart.Shared.Events;

namespace SpendSmart.Budget.API.Consumers
{

    public class BudgetCheckConsumer : IConsumer<ExpenseAddedEvent>
    {
        private readonly IBudgetService _budgetService;

        public BudgetCheckConsumer(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        public async Task Consume(ConsumeContext<ExpenseAddedEvent> context)
        {
            var msg = context.Message;
            await _budgetService.CheckBudgetOnExpenseAsync(msg.UserId, msg.CategoryId, msg.Amount);
        }
    }

}
