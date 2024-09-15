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
        private ObservableCollection<Transaction> _transactions = [];

        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnPropertyChanged();
            }
        }

        public string NewDescription { get; set; } = string.Empty;
        public decimal NewAmount { get; set; }
        public string NewCategory { get; set; } = string.Empty;

        public ICommand AddTransactionCommand { get; }

        public TransactionsViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            AddTransactionCommand = new Command(AddTransaction);
            _ = LoadTransactions();
        }

        private async Task LoadTransactions()
        {
            IEnumerable<Transaction> transactions = await _transactionService.GetTransactionsAsync();
            Transactions = new ObservableCollection<Transaction>(transactions);
        }

        private async void AddTransaction()
        {
            Transaction newTransaction = new()
            {
                Description = NewDescription,
                Amount = NewAmount,
                Date = DateTime.Now,
                Category = NewCategory
            };
            await _transactionService.AddTransactionAsync(newTransaction);
            Transactions.Add(newTransaction);

            NewDescription = string.Empty;
            NewAmount = 0;
            NewCategory = string.Empty;
            OnPropertyChanged(nameof(NewDescription));
            OnPropertyChanged(nameof(NewAmount));
            OnPropertyChanged(nameof(NewCategory));
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
