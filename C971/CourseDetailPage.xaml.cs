using Plugin.LocalNotification;

namespace C971
{
    [QueryProperty(nameof(CourseId), "courseId")]
    [QueryProperty(nameof(TermId), "termId")]
    public partial class CourseDetailPage : ContentPage
    {
        private readonly LocalDbService _db;
        private Models.Course _course = new();

        public string CourseId { get; set; } = "0";
        public string TermId { get; set; } = "0";

        public CourseDetailPage(LocalDbService db)
        {
            InitializeComponent();
            _db = db;
            StatusPicker.ItemsSource = Models.Course.StatusOptions;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCourseAsync();
            await LoadAssessmentsAsync();
            ShowTab(true);
        }

        private void ShowTab(bool showDetails)
        {
            DetailsView.IsVisible = showDetails;
            AssessmentsView.IsVisible = !showDetails;

            DetailsTabBtn.BackgroundColor = showDetails ? Color.FromArgb("#512BD4") : Colors.Gray;
            AssessmentsTabBtn.BackgroundColor = showDetails ? Colors.Gray : Color.FromArgb("#512BD4");
        }

        private void OnDetailsTabClicked(object? sender, EventArgs e) => ShowTab(true);
        private void OnAssessmentsTabClicked(object? sender, EventArgs e) => ShowTab(false);

        private async Task LoadCourseAsync()
        {
            var id = int.Parse(CourseId);

            if (id != 0)
            {
                var existing = await _db.GetCourseAsync(id);
                if (existing != null)
                    _course = existing;
            }
            else
            {
                _course.TermId = int.Parse(TermId);
            }

            TitleEntry.Text = _course.Title;
            StartDatePicker.Date = _course.StartDate;
            EndDatePicker.Date = _course.EndDate;
            StatusPicker.SelectedItem = _course.Status;
            InstructorNameEntry.Text = _course.InstructorName;
            InstructorPhoneEntry.Text = _course.InstructorPhone;
            InstructorEmailEntry.Text = _course.InstructorEmail;
            NotesEditor.Text = _course.Notes;
        }

        private async Task LoadAssessmentsAsync()
        {
            if (_course.Id == 0)
            {
                AssessmentsList.ItemsSource = null;
                AddAssessmentButton.IsEnabled = false;
                return;
            }

            var assessments = await _db.GetAssessmentsForCourseAsync(_course.Id);
            AssessmentsList.ItemsSource = assessments;
            AddAssessmentButton.IsEnabled = assessments.Count < 2; // max 2: one objective, one performance
        }

        private async void OnSaveCourseClicked(object? sender, EventArgs e)
        {
            if (!ValidationHelper.IsValidTitle(TitleEntry.Text))
            {
                await DisplayAlert("Invalid Title", "Course title must be at least 2 characters.", "OK");
                return;
            }

            if (StatusPicker.SelectedItem == null)
            {
                await DisplayAlert("Missing Status", "Please select a course status.", "OK");
                return;
            }

            if (!ValidationHelper.IsValidName(InstructorNameEntry.Text))
            {
                await DisplayAlert("Invalid Name", "Instructor name can only contain letters, spaces, hyphens, apostrophes, or periods.", "OK");
                return;
            }

            if (!ValidationHelper.IsValidPhone(InstructorPhoneEntry.Text))
            {
                await DisplayAlert("Invalid Phone", "Enter a 10 digit phone number (e.g. 555-123-4567).", "OK");
                return;
            }

            if (!ValidationHelper.IsValidEmail(InstructorEmailEntry.Text))
            {
                await DisplayAlert("Invalid Email", "Enter a valid email address (e.g. name@example.com).", "OK");
                return;
            }

            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Invalid Dates", "The end date can't be before the start date.", "OK");
                return;
            }

            _course.Title = TitleEntry.Text.Trim();
            _course.StartDate = StartDatePicker.Date;
            _course.EndDate = EndDatePicker.Date;
            _course.Status = StatusPicker.SelectedItem?.ToString() ?? "Plan to Take";
            _course.InstructorName = InstructorNameEntry.Text.Trim();
            _course.InstructorPhone = InstructorPhoneEntry.Text.Trim();
            _course.InstructorEmail = InstructorEmailEntry.Text.Trim();
            _course.Notes = NotesEditor.Text?.Trim() ?? "";

            await _db.SaveCourseAsync(_course);
            await LoadAssessmentsAsync();

            await DisplayAlert("Saved", "Course saved.", "OK");
        }

        private async void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (_course.Id == 0)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            var confirm = await DisplayAlert("Delete Course",
                $"Delete \"{_course.Title}\" and all of its assessments?",
                "Delete", "Cancel");

            if (!confirm) return;

            await _db.DeleteCourseAsync(_course);
            await Shell.Current.GoToAsync("..");
        }

        private async void OnAddAssessmentClicked(object? sender, EventArgs e)
        {
            if (_course.Id == 0)
            {
                await DisplayAlert("Save First", "Save the course before adding assessments.", "OK");
                return;
            }

            var existing = await _db.GetAssessmentsForCourseAsync(_course.Id);
            bool hasObjective = existing.Any(a => a.Type == "Objective");
            bool hasPerformance = existing.Any(a => a.Type == "Performance");

            string type;
            if (!hasObjective)
                type = "Objective";
            else if (!hasPerformance)
                type = "Performance";
            else
            {
                await DisplayAlert("Limit Reached", "A course can only have one Objective and one Performance assessment.", "OK");
                return;
            }

            await Shell.Current.GoToAsync(
                $"{nameof(AssessmentDetailPage)}?assessmentId=0&courseId={_course.Id}&assessmentType={Uri.EscapeDataString(type)}");
        }

        private async void OnAssessmentTapped(object? sender, TappedEventArgs e)
        {
            if (sender is not Border border || border.BindingContext is not Models.Assessment assessment)
                return;

            await Shell.Current.GoToAsync(
                $"{nameof(AssessmentDetailPage)}?assessmentId={assessment.Id}&courseId={_course.Id}&assessmentType={Uri.EscapeDataString(assessment.Type)}");
        }

        private async void OnStartAlertToggled(object? sender, ToggledEventArgs e)
        {
            if (_course.Id == 0)
            {
                await DisplayAlert("Save First", "Save the course before setting alerts.", "OK");
                StartAlertSwitch.IsToggled = false;
                return;
            }

            if (e.Value)
                await ScheduleNotificationAsync(1000 + _course.Id, "Course Starting",
                    $"{_course.Title} starts today!", _course.StartDate);
            else
                LocalNotificationCenter.Current.Cancel(1000 + _course.Id);
        }

        private async void OnEndAlertToggled(object? sender, ToggledEventArgs e)
        {
            if (_course.Id == 0)
            {
                await DisplayAlert("Save First", "Save the course before setting alerts.", "OK");
                EndAlertSwitch.IsToggled = false;
                return;
            }

            if (e.Value)
                await ScheduleNotificationAsync(2000 + _course.Id, "Course Ending",
                    $"{_course.Title} ends today!", _course.EndDate);
            else
                LocalNotificationCenter.Current.Cancel(2000 + _course.Id);
        }

        private async Task ScheduleNotificationAsync(int notificationId, string title, string body, DateTime notifyDate)
        {
            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = body,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyDate.Date.AddHours(8)
                }
            };

            await LocalNotificationCenter.Current.Show(request);
            await DisplayAlert("Alert Set", $"You'll be notified on {notifyDate:MM/dd/yyyy}.", "OK");
        }

        private async void OnShareNotesClicked(object? sender, EventArgs e)
        {
            var notes = NotesEditor.Text;
            if (string.IsNullOrWhiteSpace(notes))
            {
                await DisplayAlert("No Notes", "There are no notes to share.", "OK");
                return;
            }

            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = $"Notes for {_course.Title}",
                Text = notes
            });
        }
    }
}
