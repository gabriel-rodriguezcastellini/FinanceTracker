using FinanceTracker.Mobile.ViewModels;

namespace FinanceTracker.Mobile.Views
{
    public partial class ChartPage : ContentPage
    {
        private readonly ChartViewModel _viewModel;

        public ChartPage()
        {
            InitializeComponent();
            if (App.TransactionRepository == null)
            {
                throw new ArgumentNullException("App.TransactionRepository", "TransactionRepository cannot be null");
            }
            _viewModel = new ChartViewModel(App.TransactionRepository);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadChartDataAsync();
        }
    }
}
