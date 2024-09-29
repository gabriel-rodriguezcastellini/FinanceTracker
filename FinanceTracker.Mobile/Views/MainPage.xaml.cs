using FinanceTracker.Mobile.ViewModels;
using FinanceTracker.Shared.Models;

namespace FinanceTracker.Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly TransactionsViewModel _viewModel;

        public MainPage(TransactionsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
            _viewModel.TransactionAdded += OnTransactionAdded;
        }

        private void OnTransactionAdded(object? sender, Transaction e)
        {
            TransactionsCollectionView.ScrollTo(e, position: ScrollToPosition.Start, animate: true);
        }
    }
}
