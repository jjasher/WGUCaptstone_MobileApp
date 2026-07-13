using SQLite;

namespace C971.Models
{
    [Table("Courses")]
    public class Course : ScheduledItem
    {
        public int TermId { get; set; }

        [NotNull]
        public string Title { get; set; } = "";

        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(3);

        public string Status { get; set; } = "Plan to Take";

        public string InstructorName { get; set; } = "";

        public string Notes { get; set; } = "";

        public static readonly string[] StatusOptions =
        {
            "Plan to Take",
            "In Progress",
            "Completed",
            "Dropped"
        };

        public override string DisplayName => Title;

        public override string GetSummary()
            => $"{Title} ({Status}): {StartDate:MM/dd/yyyy} – {EndDate:MM/dd/yyyy}";
    }
}
