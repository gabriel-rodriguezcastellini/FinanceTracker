using FinanceTracker.Data.Repositories;
using FinanceTracker.Shared.Models;

namespace FinanceTracker.Core.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task AddCategoryAsync(Category newCategory);
        Task<Category> GetOrCreateCategoryAsync(string newCategory);
    }

    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await categoryRepository.GetCategoriesAsync();
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {
            return await categoryRepository.GetCategoryByNameAsync(categoryName);
        }

        public async Task AddCategoryAsync(Category newCategory)
        {
            await categoryRepository.AddCategoryAsync(newCategory);
        }

        public async Task<Category> GetOrCreateCategoryAsync(string newCategory)
        {
            Category? category = await categoryRepository.GetCategoryByNameAsync(newCategory);
            if (category == null)
            {
                category = new Category { Name = newCategory };
                await categoryRepository.AddCategoryAsync(category);
            }
            return category;
        }
    }
}
