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
            ConfigureServices(services);
            MainPage = new NavigationPage(services.BuildServiceProvider().GetRequiredService<MainPage>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddDbContext<FinanceTrackerDbContext>(options => options.UseSqlServer(new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection")));
            _ = services.AddSingleton<ITransactionRepository, TransactionRepository>();
            _ = services.AddSingleton<ITransactionService, TransactionService>();
            _ = services.AddSingleton<TransactionsViewModel>();
            _ = services.AddSingleton<MainPage>();
        }
    }
}
