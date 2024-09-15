using FinanceTracker.Core.Services;
using FinanceTracker.Data;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Mobile.ViewModels;
using FinanceTracker.Mobile.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            ServiceCollection services = new();
            App.ConfigureServices(services);

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            MainPage = new NavigationPage(serviceProvider.GetRequiredService<MainPage>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string connectionString = GetConnectionString();
            _ = services.AddDbContext<FinanceTrackerDbContext>(options =>
                options.UseSqlServer(connectionString));

            _ = services.AddSingleton<ITransactionRepository, TransactionRepository>();
            _ = services.AddSingleton<ITransactionService, TransactionService>();
            _ = services.AddSingleton<TransactionsViewModel>();
            _ = services.AddSingleton<MainPage>();
        }

        private static string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string? connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
            string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new InvalidOperationException("Database password not found in environment variables.");

            return connectionString.Replace("{DB_PASSWORD}", dbPassword);
        }
    }
}
