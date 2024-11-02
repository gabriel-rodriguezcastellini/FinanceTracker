using FinanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task AddCategoryAsync(Category newCategory);
        Task DeleteCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
    }

    public class CategoryRepository(FinanceTrackerDbContext context) : ICategoryRepository
    {
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {
            return (await context.Categories.ToListAsync()).Find(c => c.Name == categoryName);
        }

        public async Task AddCategoryAsync(Category newCategory)
        {
            _ = await context.Categories.AddAsync(newCategory);
            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _ = context.Categories.Remove(category);
            _ = await context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _ = context.Categories.Update(category);
            _ = await context.SaveChangesAsync();
        }
    }
}
