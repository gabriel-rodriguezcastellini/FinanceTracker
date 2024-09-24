using FinanceTracker.Core.Services;
using FinanceTracker.Data;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Mobile.ViewModels;
using FinanceTracker.Mobile.Views;

namespace FinanceTracker.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            _ = builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    _ = fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    _ = fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            _ = builder.Services.AddDbContext<FinanceTrackerDbContext>();
            _ = builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
            _ = builder.Services.AddSingleton<ITransactionService, TransactionService>();
            _ = builder.Services.AddSingleton<TransactionsViewModel>();
            _ = builder.Services.AddSingleton<MainPage>();
            return builder.Build();
        }
    }
}
