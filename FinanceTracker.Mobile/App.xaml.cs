using FinanceTracker.Data.Repositories;

namespace FinanceTracker.Mobile
{
    public partial class App : Application
    {
        public static ITransactionRepository? TransactionRepository { get; private set; }

        public App(ITransactionRepository transactionRepository)
        {
            InitializeComponent();

            TransactionRepository = transactionRepository;

            MainPage = new AppShell();
        }
    }
}
