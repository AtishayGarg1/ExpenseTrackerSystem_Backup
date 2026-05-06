using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Budget.API.Entities
{
    public class BudgetEntity
    {
        [Key]
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        
        // Null means this is a global budget for the user
        public int? CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal LimitAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public string Currency { get; set; } = "USD";
        
        // e.g. MONTHLY/WEEKLY/CUSTOM
        public string Period { get; set; } = "MONTHLY"; 
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Computed property
        public decimal GetRemainingAmount() => LimitAmount - SpentAmount;
    }
}
