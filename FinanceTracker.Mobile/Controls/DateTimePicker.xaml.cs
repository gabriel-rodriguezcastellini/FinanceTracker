namespace FinanceTracker.Mobile.Controls
{
    public partial class DateTimePicker : ContentView
    {
        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(DateTimePicker), DateTime.Now);

        public static readonly BindableProperty SelectedTimeProperty =
            BindableProperty.Create(nameof(SelectedTime), typeof(TimeSpan?), typeof(DateTimePicker), TimeSpan.Zero);

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public TimeSpan? SelectedTime
        {
            get => (TimeSpan?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public DateTimePicker()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}
