namespace SpendSmart.Shared.Events
{
    public interface BudgetThresholdReachedEvent
    {
        int UserId { get; }
        int BudgetId { get; }
        string BudgetName { get; }
        string Type { get; } // WARNING or LIMIT_REACHED
        decimal Percentage { get; }
        decimal SpentAmount { get; }
        decimal LimitAmount { get; }
    }
}
