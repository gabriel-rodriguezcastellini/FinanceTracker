using FinanceTracker.Core.Services;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Shared.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.Tests
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task AddTransaction_ShouldAddTransaction()
        {
            // Arrange
            Mock<ITransactionRepository> mockRepo = new();
            Mock<ILogger<TransactionService>> mockLogger = new();
            TransactionService service = new(mockRepo.Object, mockLogger.Object);
            Transaction transaction = new() { Id = 1, Amount = 100, Description = "Test", Date = DateTime.Now, Category = "Test" };

            // Act
            await service.AddTransactionAsync(transaction);

            // Assert
            mockRepo.Verify(repo => repo.AddTransactionAsync(transaction), Times.Once);
        }
    }
}
