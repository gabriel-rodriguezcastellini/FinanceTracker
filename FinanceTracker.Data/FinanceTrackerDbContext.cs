using FinanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data
{
    public class FinanceTrackerDbContext(DbContextOptions<FinanceTrackerDbContext> options) : DbContext(options)
    {
        public DbSet<Transaction> Transactions { get; set; }
    }
}
