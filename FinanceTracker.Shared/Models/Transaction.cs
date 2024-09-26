using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Shared.Models
{
    public class Transaction
    {
        [Required]
        public int Id { get; set; }

        public required string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public required string Category { get; set; }
    }
}
