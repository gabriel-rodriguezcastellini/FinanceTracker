using FinanceTracker.Core.Services;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Shared.Models;
using Moq;

namespace FinanceTracker.Tests
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task AddCategory_ShouldAddCategory()
        {
            // Arrange
            Mock<ICategoryRepository> mockRepo = new();
            CategoryService service = new(mockRepo.Object);
            Category category = new() { Id = 1, Name = "Test" };

            // Act
            await service.AddCategoryAsync(category);

            // Assert
            mockRepo.Verify(repo => repo.AddCategoryAsync(category), Times.Once);
        }

        [Fact]
        public async Task GetCategoryByName_ShouldReturnCategory()
        {
            // Arrange
            Mock<ICategoryRepository> mockRepo = new();
            CategoryService service = new(mockRepo.Object);
            Category category = new() { Id = 1, Name = "Test" };
            _ = mockRepo.Setup(repo => repo.GetCategoryByNameAsync("Test")).ReturnsAsync(category);

            // Act
            Category? result = await service.GetCategoryByNameAsync("Test");

            // Assert
            Assert.Equal(category, result);
        }

        [Fact]
        public async Task GetOrCreateCategory_ShouldReturnExistingCategory()
        {
            // Arrange
            Mock<ICategoryRepository> mockRepo = new();
            CategoryService service = new(mockRepo.Object);
            Category category = new() { Id = 1, Name = "Test" };
            _ = mockRepo.Setup(repo => repo.GetCategoryByNameAsync("Test")).ReturnsAsync(category);

            // Act
            Category result = await service.GetOrCreateCategoryAsync("Test");

            // Assert
            Assert.Equal(category, result);
            mockRepo.Verify(repo => repo.AddCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task GetOrCreateCategory_ShouldCreateNewCategory()
        {
            // Arrange
            Mock<ICategoryRepository> mockRepo = new();
            CategoryService service = new(mockRepo.Object);
            _ = mockRepo.Setup(repo => repo.GetCategoryByNameAsync("NewCategory")).ReturnsAsync((Category?)null);

            // Act
            Category result = await service.GetOrCreateCategoryAsync("NewCategory");

            // Assert
            Assert.Equal("NewCategory", result.Name);
            mockRepo.Verify(repo => repo.AddCategoryAsync(It.Is<Category>(c => c.Name == "NewCategory")), Times.Once);
        }
    }
}
