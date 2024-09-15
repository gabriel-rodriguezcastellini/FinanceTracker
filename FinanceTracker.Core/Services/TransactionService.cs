using FinanceTracker.Data.Repositories;
using FinanceTracker.Shared.Models;

namespace FinanceTracker.Core.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
    }

    public class TransactionService(ITransactionRepository repository) : ITransactionService
    {
        private readonly ITransactionRepository _repository = repository;

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            return await _repository.GetTransactionsAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _repository.AddTransactionAsync(transaction);
        }
    }
}
