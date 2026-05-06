using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Income.API.Entities
{
    public class Income
    {
        [Key]
        public int IncomeId { get; set; }
        public int UserId { get; set; }
        
        // e.g. SALARY/FREELANCE/INVESTMENT/RENTAL/OTHER
        public string Source { get; set; } = string.Empty; 
        
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        
        public bool IsRecurring { get; set; } = false;
        
        // e.g. MONTHLY/WEEKLY/YEARLY
        public string? RecurrenceType { get; set; } 
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
