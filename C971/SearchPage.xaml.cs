namespace C971
{
    [QueryProperty(nameof(Query), "query")]
    public partial class SearchPage : ContentPage
    {
        private readonly LocalDbService _db;

        public string Query { get; set; } = "";

        public SearchPage(LocalDbService db)
        {
            InitializeComponent();
            _db = db;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrWhiteSpace(Query))
            {
                SearchEntry.Text = Uri.UnescapeDataString(Query);
                await RunSearchAsync(SearchEntry.Text);
            }
        }

        private async void OnSearchClicked(object? sender, EventArgs e)
        {
            await RunSearchAsync(SearchEntry.Text);
        }

        private async Task RunSearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ResultsList.ItemsSource = null;
                ResultCountLabel.IsVisible = false;
                NoResultsLabel.IsVisible = false;
                return;
            }

            var results = await _db.SearchCoursesAsync(query);

            ResultsList.ItemsSource = results;
            NoResultsLabel.IsVisible = results.Count == 0;
            ResultCountLabel.IsVisible = results.Count > 0;
            ResultCountLabel.Text = $"{results.Count} result{(results.Count == 1 ? "" : "s")} found";
        }

        private async void OnCourseTapped(object? sender, TappedEventArgs e)
        {
            if (sender is not Border border || border.BindingContext is not Models.Course course)
                return;

            await Shell.Current.GoToAsync(
                $"{nameof(CourseDetailPage)}?courseId={course.Id}&termId={course.TermId}");
        }
    }
}
