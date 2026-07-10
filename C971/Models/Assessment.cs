using SQLite;

namespace C971.Models
{
    [Table("Assessments")]
    public class Assessment : ScheduledItem
    {
        public int CourseId { get; set; }

        [NotNull]
        public string Name { get; set; } = "";

        public string Type { get; set; } = "Objective";

        public DateTime DueDate { get; set; } = DateTime.Today.AddMonths(3);

        public static readonly string[] TypeOptions = { "Objective", "Performance" };

        public override string DisplayName => Name;

        public override string GetSummary()
            => $"{Name} ({Type}): Due {DueDate:MM/dd/yyyy}";
    }
}
