using FinanceTracker.Core;
using FinanceTracker.Core.Services;
using FinanceTracker.Data;
using FinanceTracker.Data.Repositories;
using FinanceTracker.Mobile.ViewModels;
using FinanceTracker.Mobile.Views;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FinanceTracker.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            _ = builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    _ = fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    _ = fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            _ = builder.Services.AddDbContext<FinanceTrackerDbContext>();
            _ = builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
            _ = builder.Services.AddSingleton<ITransactionService, TransactionService>();
            _ = builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
            _ = builder.Services.AddSingleton<ICategoryService, CategoryService>();
            _ = builder.Services.AddSingleton<TransactionsViewModel>();
            _ = builder.Services.AddTransient<ChartViewModel>();
            _ = builder.Services.AddTransient<ChartPage>();
            _ = builder.Services.AddSingleton<MainPage>();
            _ = builder.Services.AddSingleton<ExceptionLogger>();
            _ = builder.Services.AddTransient<CategoriesViewModel>();
            _ = builder.Services.AddTransient<CategoriesPage>();

            MauiApp app = builder.Build();

            ExceptionLogger exceptionLogger = app.Services.GetRequiredService<ExceptionLogger>();
            GlobalExceptionHandler.Initialize(exceptionLogger);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    GlobalExceptionHandler.HandleException(ex);
                }
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                GlobalExceptionHandler.HandleException(e.Exception);
                e.SetObserved();
            };

            return app;
        }
    }

    public static class GlobalExceptionHandler
    {
        private static ExceptionLogger? _exceptionLogger;

        public static void Initialize(ExceptionLogger exceptionLogger)
        {
            _exceptionLogger = exceptionLogger;
        }

        public static void HandleException(Exception ex)
        {
            _exceptionLogger?.LogException(ex);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
                }
            });
        }
    }
}
