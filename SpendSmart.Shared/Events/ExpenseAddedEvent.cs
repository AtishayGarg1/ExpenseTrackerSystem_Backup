namespace SpendSmart.Shared.Events
{
    public interface ExpenseAddedEvent
    {
        int ExpenseId { get; }
        int UserId { get; }
        int CategoryId { get; }
        decimal Amount { get; }
        DateTime Date { get; }
    }
}
