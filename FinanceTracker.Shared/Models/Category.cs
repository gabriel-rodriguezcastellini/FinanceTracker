using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Shared.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
