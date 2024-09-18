using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Data
{
    public class FinanceTrackerDbContextFactory : IDesignTimeDbContextFactory<FinanceTrackerDbContext>
    {
        public FinanceTrackerDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            DbContextOptionsBuilder<FinanceTrackerDbContext> optionsBuilder = new();
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (!string.IsNullOrEmpty(dbPassword))
            {
                connectionString += $"Password={dbPassword};";
            }

            _ = optionsBuilder.UseSqlServer(connectionString);
            return new FinanceTrackerDbContext(optionsBuilder.Options);
        }
    }
}
