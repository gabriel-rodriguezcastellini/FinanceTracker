using FinanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task AddTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByCategoryAsync(int id);
    }

    public class TransactionRepository(FinanceTrackerDbContext context) : ITransactionRepository
    {
        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            List<Transaction> transactions = await context.Transactions.ToListAsync();
            return transactions.OrderByDescending(t => t.Date);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            List<Transaction> transactions = await context.Transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();
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

        public async Task<IEnumerable<Transaction>> GetTransactionsByCategoryAsync(int id)
        {
            List<Transaction> transactions = await context.Transactions
                .Where(t => t.CategoryId == id)
                .ToListAsync();
            return transactions.OrderByDescending(t => t.Date);
        }
    }
}
