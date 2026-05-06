namespace SpendSmart.Shared.Events
{
    public interface UserRegisteredEvent
    {
        int UserId { get; }
        string Email { get; }
        string FullName { get; }
        string PreferredCurrency { get; }
    }
}
