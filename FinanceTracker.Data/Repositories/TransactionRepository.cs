using FinanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
    }

    public class TransactionRepository(FinanceTrackerDbContext context) : ITransactionRepository
    {
        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            return await context.Transactions.ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _ = await context.Transactions.AddAsync(transaction);
            _ = await context.SaveChangesAsync();
        }
    }
}
