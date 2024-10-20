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
            Category category = new() { Id = 1, Name = "Test" };
            Transaction transaction = new() { Id = 1, Amount = 100, Description = "Test", Date = DateTime.Now, Category = category };

            // Act
            await service.AddTransactionAsync(transaction);

            // Assert
            mockRepo.Verify(repo => repo.AddTransactionAsync(transaction), Times.Once);
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnTransactions()
        {
            // Arrange
            Mock<ITransactionRepository> mockRepo = new();
            Mock<ILogger<TransactionService>> mockLogger = new();
            TransactionService service = new(mockRepo.Object, mockLogger.Object);
            List<Transaction> transactions =
            [
                new Transaction { Id = 1, Amount = 100, Description = "Test1", Date = DateTime.Now, Category = new Category { Id = 1, Name = "Test" } },
                new Transaction { Id = 2, Amount = 200, Description = "Test2", Date = DateTime.Now, Category = new Category { Id = 2, Name = "Test2" } }
            ];
            _ = mockRepo.Setup(repo => repo.GetTransactionsAsync()).ReturnsAsync(transactions);

            // Act
            IEnumerable<Transaction> result = await service.GetTransactionsAsync();

            // Assert
            Assert.Equal(transactions, result);
        }

        [Fact]
        public async Task UpdateTransaction_ShouldUpdateTransaction()
        {
            // Arrange
            Mock<ITransactionRepository> mockRepo = new();
            Mock<ILogger<TransactionService>> mockLogger = new();
            TransactionService service = new(mockRepo.Object, mockLogger.Object);
            Category category = new() { Id = 1, Name = "Test" };
            Transaction transaction = new() { Id = 1, Amount = 100, Description = "Test", Date = DateTime.Now, Category = category };

            // Act
            await service.UpdateTransactionAsync(transaction);

            // Assert
            mockRepo.Verify(repo => repo.UpdateTransactionAsync(transaction), Times.Once);
        }
    }
}
