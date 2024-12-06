using FinanceTracker.Mobile.ViewModels;

namespace FinanceTracker.Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(TransactionsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
