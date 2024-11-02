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
        private DateTime? _selectedDate;
        private TimeSpan? _selectedTime;
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
                _selectedTransaction = value ?? new Transaction { Description = string.Empty };
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

        public ICommand AddTransactionCommand { get; }
        public ICommand ToggleFormVisibilityCommand { get; }
        public ICommand EditTransactionCommand { get; }
        public ICommand SaveTransactionCommand { get; }
        public ICommand CancelEditTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand FilterTransactionsCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public TransactionsViewModel(ITransactionService transactionService, ICategoryService categoryService, ExceptionLogger exceptionLogger)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _exceptionLogger = exceptionLogger;
            _transactions = [];
            _categories = [];

            StartDate = DateTime.Now;
            EndDate = DateTime.Now;

            _selectedTransaction = new Transaction { Description = string.Empty };
            AddTransactionCommand = new Command(AddTransaction);
            EditTransactionCommand = new Command<Transaction>(EditTransaction);
            DeleteTransactionCommand = new Command<Transaction>(DeleteTransaction);
            ToggleFormVisibilityCommand = new Command(ToggleFormVisibility);
            SaveTransactionCommand = new Command(SaveTransaction);
            CancelEditTransactionCommand = new Command(CancelEditTransaction);
            FilterTransactionsCommand = new Command(FilterTransactions);
            ClearFiltersCommand = new Command(ClearFilters);

            _ = LoadTransactions();
            _ = LoadCategories();
        }

        private void ClearFilters()
        {
            StartDate = Transactions.Min(t => t.Date);
            EndDate = Transactions.Max(t => t.Date);
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

        private void ToggleFormVisibility()
        {
            IsFormVisible = !IsFormVisible;
        }

        public event EventHandler<Transaction>? TransactionAdded;

        private async void AddTransaction()
        {
            try
            {
                Transaction newTransaction = new()
                {
                    Description = NewDescription,
                    Amount = NewAmount,
                    Date = SelectedDate ?? DateTime.Now,
                    Category = SelectedCategory ?? throw new InvalidOperationException("Category cannot be null")
                };

                await _transactionService.AddTransactionAsync(newTransaction);
                Transactions.Insert(0, newTransaction);

                await LoadCategories();

                TransactionAdded?.Invoke(this, newTransaction);
                await LoadTransactions();
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
                SelectedTransaction = transaction;
                NewDescription = transaction.Description;
                NewAmount = transaction.Amount;
                SelectedCategory = transaction.Category;
                SelectedDate = transaction.Date;
                SelectedTime = transaction.Date.TimeOfDay;
                IsFormVisible = true;

                OnPropertyChanged(nameof(NewDescription));
                OnPropertyChanged(nameof(NewAmount));
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged(nameof(SelectedTime));
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
                if (SelectedTransaction != null)
                {
                    SelectedTransaction.Description = NewDescription;
                    SelectedTransaction.Amount = NewAmount;
                    SelectedTransaction.Date = (SelectedDate ?? DateTime.Now).Date + (SelectedTime ?? TimeSpan.Zero);
                    SelectedTransaction.Category = SelectedCategory ?? throw new InvalidOperationException("Category cannot be null");

                    await _transactionService.UpdateTransactionAsync(SelectedTransaction);
                    await LoadTransactions();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
            }
        }

        private void CancelEditTransaction()
        {
            ClearForm();
        }

        private void ClearForm()
        {
            NewDescription = string.Empty;
            NewAmount = 0;
            SelectedCategory = null;
            SelectedDate = null;
            SelectedTime = null;
            IsFormVisible = false;
            SelectedTransaction = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
