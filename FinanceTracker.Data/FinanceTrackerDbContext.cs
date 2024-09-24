using FinanceTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data
{
    public class FinanceTrackerDbContext : DbContext
    {
        public FinanceTrackerDbContext()
        {

        }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlite($"Filename={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FinanceTracker.db")}");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    _ = modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasPrecision(18, 2);
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}

