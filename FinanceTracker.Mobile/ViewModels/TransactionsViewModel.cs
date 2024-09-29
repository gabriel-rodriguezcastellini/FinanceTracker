using FinanceTracker.Core;
using FinanceTracker.Core.Services;
using FinanceTracker.Shared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FinanceTracker.Mobile.ViewModels
{
    public class TransactionsViewModel : INotifyPropertyChanged
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly ExceptionLogger _exceptionLogger;
        private ObservableCollection<Transaction> _transactions = [];
        private ObservableCollection<Category> _categories = [];
        private bool _isNewCategory;
        private bool _isFormVisible;

        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public string NewDescription { get; set; } = string.Empty;
        public decimal NewAmount { get; set; }
        public string NewCategory { get; set; } = string.Empty;
        public Category? SelectedCategory { get; set; }

        public bool IsNewCategory
        {
            get => _isNewCategory;
            set
            {
                _isNewCategory = value;
                OnPropertyChanged();
            }
        }

        public bool IsFormVisible
        {
            get => _isFormVisible;
            set
            {
                _isFormVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddTransactionCommand { get; }

        public ICommand ToggleFormVisibilityCommand { get; }

        public TransactionsViewModel(ITransactionService transactionService, ICategoryService categoryService, ExceptionLogger exceptionLogger)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _exceptionLogger = exceptionLogger;
            AddTransactionCommand = new Command(AddTransaction);
            ToggleFormVisibilityCommand = new Command(ToggleFormVisibility);
            _ = LoadTransactions();
            _ = LoadCategories();
        }

        private async Task LoadTransactions()
        {
            IEnumerable<Transaction> transactions = await _transactionService.GetTransactionsAsync();
            Transactions = new ObservableCollection<Transaction>(transactions);
        }

        private async Task LoadCategories()
        {
            IEnumerable<Category> categories = await _categoryService.GetCategoriesAsync();
            Categories = new ObservableCollection<Category>(categories);
        }

        private void ToggleFormVisibility()
        {
            IsFormVisible = !IsFormVisible;
        }

        public event EventHandler<Transaction>? TransactionAdded;

        private async void AddTransaction()
        {
            try
            {
                Category category = IsNewCategory
                    ? await _categoryService.GetOrCreateCategoryAsync(NewCategory)
                    : SelectedCategory ?? throw new InvalidOperationException("No category selected");

                Transaction newTransaction = new()
                {
                    Description = NewDescription,
                    Amount = NewAmount,
                    Date = DateTime.Now,
                    Category = category
                };

                await _transactionService.AddTransactionAsync(newTransaction);
                Transactions.Insert(0, newTransaction);

                TransactionAdded?.Invoke(this, newTransaction);

                NewDescription = string.Empty;
                NewAmount = 0;
                NewCategory = string.Empty;
                SelectedCategory = null;
                IsNewCategory = false;
                OnPropertyChanged(nameof(NewDescription));
                OnPropertyChanged(nameof(NewAmount));
                OnPropertyChanged(nameof(NewCategory));
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(IsNewCategory));
                IsFormVisible = false;
            }
            catch (InvalidOperationException ex)
            {
                _exceptionLogger.LogException(ex);
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
