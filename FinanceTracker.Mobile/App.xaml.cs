using FinanceTracker.Core.Services;
using FinanceTracker.Data;
using FinanceTracker.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Mobile
{
    public partial class App : Application
    {
        public static ITransactionRepository? TransactionRepository { get; private set; }
        public static ICategoryRepository? CategoryRepository { get; private set; }
        public static ICategoryService? CategoryService { get; private set; }

        public App(FinanceTrackerDbContext financeTrackerDbContext, ITransactionRepository transactionRepository, ICategoryRepository categoryRepository, ICategoryService categoryService)
        {
            InitializeComponent();
            financeTrackerDbContext.Database.Migrate();
            TransactionRepository = transactionRepository;
            CategoryRepository = categoryRepository;
            CategoryService = categoryService;

            MainPage = new AppShell();
        }
    }
}
