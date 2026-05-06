using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Category.API.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        
        // Null means system-default category visible to all.
        // Value means custom user category.
        public int? UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty; // emoji or icon code
        public string Color { get; set; } = "#000000"; // hex string
        
        public string Type { get; set; } = "EXPENSE"; // EXPENSE or INCOME
        
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
