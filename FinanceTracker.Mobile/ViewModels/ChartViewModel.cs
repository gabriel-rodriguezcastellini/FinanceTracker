using FinanceTracker.Data.Repositories;
using Microcharts;
using SkiaSharp;

namespace FinanceTracker.Mobile.ViewModels
{
    public partial class ChartViewModel : BaseViewModel
    {
        private readonly ITransactionRepository _transactionRepository;

        public ChartViewModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _chart = new PieChart();
            _ = LoadChartDataAsync().ConfigureAwait(false);
        }

        private Chart _chart;
        public Chart Chart
        {
            get => _chart;
            set => SetProperty(ref _chart, value);
        }

        private bool _hasTransactions;
        public bool HasTransactions
        {
            get => _hasTransactions;
            set => SetProperty(ref _hasTransactions, value);
        }

        public async Task LoadChartDataAsync()
        {
            IEnumerable<Shared.Models.Transaction> transactions = await _transactionRepository.GetTransactionsAsync();
            HasTransactions = transactions.Any();

            if (!HasTransactions)
            {
                return;
            }

            var groupedData = transactions
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                .ToList();

            decimal totalAmount = groupedData.Sum(data => data.Amount);

            List<ChartEntry> entries = groupedData.Select(data => new ChartEntry((float)data.Amount)
            {
                Label = data.Category.Name,
                ValueLabel = $"{data.Amount:C} ({data.Amount / totalAmount * 100:F2}%)",
                Color = GenerateColor(data.Category.Name)
            }).ToList();

            Chart = new PieChart
            {
                Entries = entries,
                LabelTextSize = 30,
                Margin = 20
            };
        }

        private static SKColor GenerateColor(string categoryName)
        {
            int hash = categoryName.GetHashCode();
            byte r = (byte)((hash & 0xFF0000) >> 16);
            byte g = (byte)((hash & 0x00FF00) >> 8);
            byte b = (byte)(hash & 0x0000FF);
            return new SKColor(r, g, b);
        }
    }
}
