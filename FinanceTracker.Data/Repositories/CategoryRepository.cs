using FinanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task AddCategoryAsync(Category newCategory);
    }

    public class CategoryRepository(FinanceTrackerDbContext context) : ICategoryRepository
    {
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {
            return await context.Categories
                .FirstOrDefaultAsync(c => c.Name == categoryName);
        }

        public async Task AddCategoryAsync(Category newCategory)
        {
            _ = await context.Categories.AddAsync(newCategory);
            _ = await context.SaveChangesAsync();
        }
    }
}
