using CommunityToolkit.Mvvm.Messaging;
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
        private ObservableCollection<Transaction> _transactions;
        private ObservableCollection<Category> _categories;
        private bool _isNewCategory;
        private Transaction _selectedTransaction;
        private bool _isFormVisible;
        private DateTime? _selectedDate = DateTime.Now;
        private TimeSpan? _selectedTime = TimeSpan.Zero;
        private DateTime _startDate;
        private DateTime _endDate;

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

        public Transaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value!;
                if (_selectedTransaction != null)
                {
                    NewDescription = _selectedTransaction.Description;
                    NewAmount = _selectedTransaction.Amount;
                    NewCategory = _selectedTransaction.Category?.Name ?? string.Empty;
                    SelectedCategory = _selectedTransaction.Category;
                    SelectedDate = _selectedTransaction.Date;
                    SelectedTime = _selectedTransaction.Date.TimeOfDay;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(NewDescription));
                OnPropertyChanged(nameof(NewAmount));
                OnPropertyChanged(nameof(NewCategory));
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged(nameof(SelectedTime));
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

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan? SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleFormVisibilityCommand { get; }
        public ICommand EditTransactionCommand { get; }
        public ICommand SaveTransactionCommand { get; }
        public ICommand CancelEditTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand FilterTransactionsCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand OpenFormAddTransactionCommand { get; }

        public TransactionsViewModel(ITransactionService transactionService, ICategoryService categoryService, ExceptionLogger exceptionLogger)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _exceptionLogger = exceptionLogger;

            WeakReferenceMessenger.Default.Register<CategoryDeletedMessage>(this, (_, m) => OnCategoryDeleted(m.Category));
            WeakReferenceMessenger.Default.Register<CategoryAddedMessage>(this, (_, _) => OnCategoryAdded());

            _transactions = [];
            _categories = [];
            _selectedTransaction = new Transaction { Description = string.Empty };

            EditTransactionCommand = new Command<Transaction>(EditTransaction);
            DeleteTransactionCommand = new Command<Transaction>(DeleteTransaction);
            ToggleFormVisibilityCommand = new Command(ToggleFormVisibility);
            OpenFormAddTransactionCommand = new Command(OpenFormAddTransaction);
            SaveTransactionCommand = new Command(SaveTransaction);
            CancelEditTransactionCommand = new Command(CancelEditTransaction);
            FilterTransactionsCommand = new Command(FilterTransactions);
            ClearFiltersCommand = new Command(ClearFilters);

            _ = LoadTransactions();
            _ = LoadCategories();
        }

        private async void OnCategoryAdded()
        {
            await LoadCategories();
        }

        private async void OnCategoryDeleted(Category _)
        {
            await LoadCategories();
            await LoadTransactions();
        }

        private void ClearFilters()
        {
            if (Transactions.Any())
            {
                StartDate = Transactions.Min(t => t.Date);
                EndDate = Transactions.Max(t => t.Date);
            }
            else
            {
                StartDate = DateTime.Now;
                EndDate = DateTime.Now;
            }
            _ = LoadTransactions();
        }

        private async void FilterTransactions()
        {
            try
            {
                IEnumerable<Transaction> transactions = await _transactionService.GetTransactionsByDateRangeAsync(StartDate, EndDate);
                Transactions = new ObservableCollection<Transaction>(transactions.OrderByDescending(t => t.Date));
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private async void DeleteTransaction(Transaction transaction)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(transaction);
                _ = Transactions.Remove(transaction);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private async Task LoadTransactions()
        {
            try
            {
                IEnumerable<Transaction> transactions = await _transactionService.GetTransactionsAsync();
                Transactions = new ObservableCollection<Transaction>(transactions.OrderByDescending(t => t.Date));

                if (Transactions.Any())
                {
                    StartDate = Transactions.Min(t => t.Date);
                    EndDate = Transactions.Max(t => t.Date);
                }
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private async Task LoadCategories()
        {
            try
            {
                IEnumerable<Category> categories = await _categoryService.GetCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private async void OpenFormAddTransaction()
        {
            try
            {
                if (!Categories.Any())
                {
                    await (Application.Current?.MainPage?.DisplayAlert("Warning", "Please, add a new category first.", "OK") ?? Task.CompletedTask);
                    return;
                }

                IsFormVisible = true;

                ClearForm();
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private void EditTransaction(Transaction transaction)
        {
            try
            {
                ClearForm();
                SelectedTransaction = transaction;
                IsFormVisible = true;
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private async void SaveTransaction()
        {
            try
            {
                if (SelectedTransaction == null)
                {
                    Transaction newTransaction = new()
                    {
                        Description = NewDescription,
                        Amount = NewAmount,
                        Date = (SelectedDate ?? DateTime.Now).Date + (SelectedTime ?? TimeSpan.Zero),
                        Category = SelectedCategory ?? throw new InvalidOperationException("Category cannot be null"),
                        CategoryId = SelectedCategory.Id
                    };

                    await _transactionService.AddTransactionAsync(newTransaction);
                    Transactions.Add(newTransaction);
                }
                else
                {
                    SelectedTransaction.Description = NewDescription;
                    SelectedTransaction.Amount = NewAmount;
                    SelectedTransaction.Date = (SelectedDate ?? DateTime.Now).Date + (SelectedTime ?? TimeSpan.Zero);
                    SelectedTransaction.Category = SelectedCategory ?? throw new InvalidOperationException("Category cannot be null");
                    SelectedTransaction.CategoryId = SelectedCategory.Id;

                    await _transactionService.UpdateTransactionAsync(SelectedTransaction);
                }

                IsFormVisible = false;
                ClearForm();
                await LoadTransactions();
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private void CancelEditTransaction()
        {
            IsFormVisible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            NewDescription = string.Empty;
            NewAmount = 0;
            NewCategory = string.Empty;
            SelectedCategory = null;
            SelectedDate = DateTime.Now;
            SelectedTime = TimeSpan.Zero;
            SelectedTransaction = null;

            OnPropertyChanged(nameof(NewDescription));
            OnPropertyChanged(nameof(NewAmount));
            OnPropertyChanged(nameof(NewCategory));
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(SelectedDate));
            OnPropertyChanged(nameof(SelectedTime));
            OnPropertyChanged(nameof(IsFormVisible));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ToggleFormVisibility()
        {
            IsFormVisible = !IsFormVisible;
        }
    }
}
