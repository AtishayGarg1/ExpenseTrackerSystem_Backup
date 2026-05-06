using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Expense.API.Entities
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        
        // CASH/CARD/UPI/NET_BANKING/WALLET
        public string PaymentMode { get; set; } = "CARD"; 
        
        public string? ReceiptUrl { get; set; }
        public string Tags { get; set; } = string.Empty; // Comma separated tags
        
        public bool IsRecurring { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
