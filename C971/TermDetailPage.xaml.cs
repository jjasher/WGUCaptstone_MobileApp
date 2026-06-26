namespace C971
{
    [QueryProperty(nameof(TermId), "termId")]
    public partial class TermDetailPage : ContentPage
    {
        private readonly LocalDbService _db;
        private Models.Term _term = new();

        public string TermId { get; set; } = "0";

        public TermDetailPage(LocalDbService db)
        {
            InitializeComponent();
            _db = db;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTermAsync();
            await LoadCoursesAsync();
            ShowTab(true);
        }

        private void ShowTab(bool showDetails)
        {
            DetailsView.IsVisible = showDetails;
            CoursesView.IsVisible = !showDetails;

            DetailsTabBtn.BackgroundColor = showDetails ? Color.FromArgb("#512BD4") : Colors.Gray;
            CoursesTabBtn.BackgroundColor = showDetails ? Colors.Gray : Color.FromArgb("#512BD4");
        }

        private void OnDetailsTabClicked(object? sender, EventArgs e) => ShowTab(true);
        private void OnCoursesTabClicked(object? sender, EventArgs e) => ShowTab(false);

        private async Task LoadTermAsync()
        {
            var id = int.Parse(TermId);

            if (id != 0)
            {
                var existing = await _db.GetTermAsync(id);
                if (existing != null)
                    _term = existing;
            }

            TitleEntry.Text = _term.Title;
            StartDatePicker.Date = _term.StartDate;
            EndDatePicker.Date = _term.EndDate;
        }

        private async Task LoadCoursesAsync()
        {
            if (_term.Id == 0)
            {
                CoursesList.ItemsSource = null;
                AddCourseButton.IsEnabled = false;
                return;
            }

            var courses = await _db.GetCoursesForTermAsync(_term.Id);
            CoursesList.ItemsSource = courses;

            var atLimit = courses.Count >= 6;
            AddCourseButton.IsEnabled = !atLimit;
            CourseLimitLabel.IsVisible = atLimit;
        }

        private async void OnSaveTermClicked(object? sender, EventArgs e)
        {
            if (!ValidationHelper.IsValidTitle(TitleEntry.Text))
            {
                await DisplayAlert("Invalid Title", "Term title must be at least 2 characters.", "OK");
                return;
            }

            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Invalid Dates", "The end date can't be before the start date.", "OK");
                return;
            }

            _term.Title = TitleEntry.Text.Trim();
            _term.StartDate = StartDatePicker.Date;
            _term.EndDate = EndDatePicker.Date;

            await _db.SaveTermAsync(_term);
            await LoadCoursesAsync();

            await DisplayAlert("Saved", "Term saved.", "OK");
        }

        private async void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (_term.Id == 0)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            var confirm = await DisplayAlert("Delete Term",
                $"Delete \"{_term.Title}\" and all of its courses and assessments?",
                "Delete", "Cancel");

            if (!confirm)
                return;

            await _db.DeleteTermAsync(_term);
            await Shell.Current.GoToAsync("..");
        }

        private async void OnAddCourseClicked(object? sender, EventArgs e)
        {
            if (_term.Id == 0)
            {
                await DisplayAlert("Save First", "Save the term before adding courses.", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(CourseDetailPage)}?courseId=0&termId={_term.Id}");
        }

        private async void OnCourseTapped(object? sender, TappedEventArgs e)
        {
            if (sender is not Border border || border.BindingContext is not Models.Course course)
                return;

            await Shell.Current.GoToAsync($"{nameof(CourseDetailPage)}?courseId={course.Id}&termId={_term.Id}");
        }
    }
}
