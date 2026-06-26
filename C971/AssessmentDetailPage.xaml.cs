using Plugin.LocalNotification;

namespace C971
{
    [QueryProperty(nameof(AssessmentId), "assessmentId")]
    [QueryProperty(nameof(CourseId), "courseId")]
    [QueryProperty(nameof(AssessmentType), "assessmentType")]
    public partial class AssessmentDetailPage : ContentPage
    {
        private readonly LocalDbService _db;
        private Models.Assessment _assessment = new();

        public string AssessmentId { get; set; } = "0";
        public string CourseId { get; set; } = "0";
        public string AssessmentType { get; set; } = "Objective";

        public AssessmentDetailPage(LocalDbService db)
        {
            InitializeComponent();
            _db = db;
            TypePicker.ItemsSource = Models.Assessment.TypeOptions;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAssessmentAsync();
        }

        private async Task LoadAssessmentAsync()
        {
            var id = int.Parse(AssessmentId);

            if (id != 0)
            {
                var existing = await _db.GetAssessmentAsync(id);
                if (existing != null)
                    _assessment = existing;
            }
            else
            {
                _assessment.CourseId = int.Parse(CourseId);
                _assessment.Type = Uri.UnescapeDataString(AssessmentType);
            }

            NameEntry.Text = _assessment.Name;
            TypePicker.SelectedItem = _assessment.Type;
            StartDatePicker.Date = _assessment.StartDate;
            DueDatePicker.Date = _assessment.DueDate;
        }

        private async void OnSaveAssessmentClicked(object? sender, EventArgs e)
        {
            if (!ValidationHelper.IsValidTitle(NameEntry.Text))
            {
                await DisplayAlert("Invalid Name", "Assessment name must be at least 2 characters.", "OK");
                return;
            }

            if (TypePicker.SelectedItem == null)
            {
                await DisplayAlert("Missing Type", "Select an assessment type (Objective or Performance).", "OK");
                return;
            }

            if (DueDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Invalid Dates", "The due date can't be before the start date.", "OK");
                return;
            }

            _assessment.Name = NameEntry.Text.Trim();
            _assessment.Type = TypePicker.SelectedItem?.ToString() ?? "Objective";
            _assessment.StartDate = StartDatePicker.Date;
            _assessment.DueDate = DueDatePicker.Date;

            await _db.SaveAssessmentAsync(_assessment);

            await DisplayAlert("Saved", "Assessment saved.", "OK");
        }

        private async void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (_assessment.Id == 0)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            var confirm = await DisplayAlert("Delete Assessment",
                $"Delete \"{_assessment.Name}\"?", "Delete", "Cancel");

            if (!confirm) return;

            await _db.DeleteAssessmentAsync(_assessment);
            await Shell.Current.GoToAsync("..");
        }

        private async void OnStartAlertToggled(object? sender, ToggledEventArgs e)
        {
            if (_assessment.Id == 0)
            {
                await DisplayAlert("Save First", "Save the assessment before setting alerts.", "OK");
                StartAlertSwitch.IsToggled = false;
                return;
            }

            if (e.Value)
                await ScheduleNotificationAsync(3000 + _assessment.Id, "Assessment Starting",
                    $"{_assessment.Name} starts today!", _assessment.StartDate);
            else
                LocalNotificationCenter.Current.Cancel(3000 + _assessment.Id);
        }

        private async void OnDueAlertToggled(object? sender, ToggledEventArgs e)
        {
            if (_assessment.Id == 0)
            {
                await DisplayAlert("Save First", "Save the assessment before setting alerts.", "OK");
                DueAlertSwitch.IsToggled = false;
                return;
            }

            if (e.Value)
                await ScheduleNotificationAsync(4000 + _assessment.Id, "Assessment Due",
                    $"{_assessment.Name} is due today!", _assessment.DueDate);
            else
                LocalNotificationCenter.Current.Cancel(4000 + _assessment.Id);
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
    }
}
