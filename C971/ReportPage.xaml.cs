namespace C971
{
    public partial class ReportPage : ContentPage
    {
        private readonly LocalDbService _db;

        public ReportPage(LocalDbService db)
        {
            InitializeComponent();
            _db = db;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadReportAsync();
        }

        private async Task LoadReportAsync()
        {
            GeneratedLabel.Text = $"Generated: {DateTime.Now:MM/dd/yyyy h:mm tt}";

            var data = await _db.GetCourseReportAsync();

            var rows = new List<ReportRow>();
            foreach (var (term, course) in data)
            {
                rows.Add(new ReportRow
                {
                    CourseName = course.Title,
                    TermName = term.Title,
                    Status = course.Status,
                    EndDate = course.EndDate
                });
            }

            ReportList.ItemsSource = rows;

            var completed = 0;
            var inProgress = 0;
            foreach (var r in rows)
            {
                if (r.Status == "Completed") completed++;
                else if (r.Status == "In Progress") inProgress++;
            }

            SummaryLabel.Text = $"{rows.Count} course{(rows.Count == 1 ? "" : "s")} total  •  {completed} completed  •  {inProgress} in progress";
        }
    }

    public class ReportRow
    {
        public string CourseName { get; set; } = "";
        public string TermName { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime EndDate { get; set; }
    }
}
