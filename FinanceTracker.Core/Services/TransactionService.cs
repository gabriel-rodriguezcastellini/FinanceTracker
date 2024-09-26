using FinanceTracker.Data.Repositories;
using FinanceTracker.Shared.Models;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Core.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
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
    }
}
