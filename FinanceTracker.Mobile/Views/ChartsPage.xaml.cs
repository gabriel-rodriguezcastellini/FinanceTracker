using FinanceTracker.Mobile.ViewModels;

namespace FinanceTracker.Mobile.Views
{
    public partial class ChartsPage : ContentPage
    {
        private readonly ChartsViewModel _viewModel;

        public ChartsPage()
        {
            InitializeComponent();
            if (App.TransactionRepository == null)
            {
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                throw new ArgumentNullException("App.TransactionRepository", "TransactionRepository cannot be null");
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            }
            _viewModel = new ChartsViewModel(App.TransactionRepository);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadChartDataAsync();
        }
    }
}
