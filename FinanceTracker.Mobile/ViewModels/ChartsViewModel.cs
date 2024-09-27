using FinanceTracker.Data.Repositories;
using Microcharts;
using SkiaSharp;

namespace FinanceTracker.Mobile.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        private readonly ITransactionRepository _transactionRepository;

        public ChartsViewModel(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _chart = new BarChart();
            _ = LoadChartDataAsync().ConfigureAwait(false);
        }

        private Chart _chart;
        public Chart Chart
        {
            get => _chart;
            set => SetProperty(ref _chart, value);
        }

        public async Task LoadChartDataAsync()
        {
            IEnumerable<Shared.Models.Transaction> transactions = await _transactionRepository.GetTransactionsAsync();
            var groupedData = transactions
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                .ToList();

            List<ChartEntry> entries = groupedData.Select(data => new ChartEntry((float)data.Amount)
            {
                Label = data.Category,
                ValueLabel = data.Amount.ToString("C"),
                Color = SKColor.Parse("#266489")
            }).ToList();

            if (entries.Count == 0)
            {
                Console.WriteLine("No data to display in the chart.");
            }
            else
            {
                Console.WriteLine($"Loaded {entries.Count} entries for the chart.");
            }

            Chart = new BarChart
            {
                Entries = entries,
                LabelTextSize = 30,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal,
                Margin = 20
            };
        }
    }
}
