using SQLite;

namespace C971.Models
{
    public abstract class ScheduledItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;

        public abstract string DisplayName { get; }

        public virtual string GetSummary()
            => $"{DisplayName} starting {StartDate:MM/dd/yyyy}";
    }
}
