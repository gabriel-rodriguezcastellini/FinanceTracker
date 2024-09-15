namespace FinanceTracker.Shared.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string Category { get; set; }
    }
}
