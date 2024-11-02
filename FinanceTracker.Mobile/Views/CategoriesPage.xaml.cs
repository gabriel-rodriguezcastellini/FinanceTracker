using FinanceTracker.Mobile.ViewModels;

namespace FinanceTracker.Mobile.Views
{
    public partial class CategoriesPage : ContentPage
    {
        public CategoriesPage(CategoriesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
