namespace C971
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _db;

        public MainPage(LocalDbService db)
        {
            InitializeComponent();
            _db = db;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTermsAsync();
        }

        private async Task LoadTermsAsync()
        {
            var terms = await _db.GetTermsAsync();
            TermsList.ItemsSource = terms;
            EmptyLabel.IsVisible = terms.Count == 0;
        }

        private async void OnAddTermClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(TermDetailPage)}?termId=0");
        }

        private async void OnSearchClicked(object? sender, EventArgs e)
        {
            var query = SearchEntry.Text?.Trim() ?? "";
            await Shell.Current.GoToAsync($"{nameof(SearchPage)}?query={Uri.EscapeDataString(query)}");
            SearchEntry.Text = "";
        }

        private async void OnTermTapped(object? sender, TappedEventArgs e)
        {
            if (sender is not Border border || border.BindingContext is not Models.Term term)
                return;

            await Shell.Current.GoToAsync($"{nameof(TermDetailPage)}?termId={term.Id}");
        }
    }
}