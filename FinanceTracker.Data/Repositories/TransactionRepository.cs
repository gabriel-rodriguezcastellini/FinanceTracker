using FinanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(Transaction transaction);
    }

    public class TransactionRepository(FinanceTrackerDbContext context) : ITransactionRepository
    {
        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            List<Transaction> transactions = await context.Transactions.ToListAsync();
            return transactions.OrderByDescending(t => t.Date);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _ = await context.Transactions.AddAsync(transaction);
            _ = await context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _ = context.Transactions.Update(transaction);
            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            _ = context.Transactions.Remove(transaction);
            _ = await context.SaveChangesAsync();
        }
    }
}
