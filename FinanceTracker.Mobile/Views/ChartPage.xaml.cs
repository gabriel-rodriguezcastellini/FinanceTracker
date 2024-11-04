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
                throw new ArgumentNullException("TransactionRepository", "TransactionRepository cannot be null");
            }
            _viewModel = new ChartViewModel(App.TransactionRepository);
            BindingContext = _viewModel;

            Button shareButton = new()
            {
                Text = "Share Chart"
            };
            shareButton.Clicked += async (sender, args) => await _viewModel.ShareChartImageAsync();

            StackLayout stackLayout = (StackLayout)Content;
            stackLayout.Children.Add(shareButton);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadChartDataAsync();
        }
    }
}
