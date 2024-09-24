using FinanceTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Mobile
{
    public partial class App : Application
    {
        public App(FinanceTrackerDbContext financeTrackerDbContext)
        {
            InitializeComponent();
            financeTrackerDbContext.Database.Migrate();
            MainPage = new AppShell();
        }
    }
}
