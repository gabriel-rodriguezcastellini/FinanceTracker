using FinanceTracker.Data.Repositories;
using FinanceTracker.Shared.Models;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Core.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(Transaction transaction);
    }

    public class TransactionService(ITransactionRepository repository, ILogger<TransactionService> logger) : ITransactionService
    {
        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            return await repository.GetTransactionsAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            logger.LogInformation("Adding transaction: {Transaction}", transaction);
            await repository.AddTransactionAsync(transaction);
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            logger.LogInformation("Updating transaction: {Transaction}", transaction);
            await repository.UpdateTransactionAsync(transaction);
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            logger.LogInformation("Deleting transaction: {Transaction}", transaction);
            await repository.DeleteTransactionAsync(transaction);
        }
    }
}
