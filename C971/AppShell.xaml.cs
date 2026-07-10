namespace C971
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(TermDetailPage), typeof(TermDetailPage));
            Routing.RegisterRoute(nameof(CourseDetailPage), typeof(CourseDetailPage));
            Routing.RegisterRoute(nameof(AssessmentDetailPage), typeof(AssessmentDetailPage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        }
    }
}