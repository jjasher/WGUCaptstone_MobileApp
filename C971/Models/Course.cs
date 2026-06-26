using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Models
{
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int TermId { get; set; }

        [NotNull]
        public string Title { get; set; } = "";

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(3);

        public string Status { get; set; } = "Plan to Take";

        public string InstructorName { get; set; } = "";
        public string InstructorPhone { get; set; } = "";
        public string InstructorEmail { get; set; } = "";

        public string Notes { get; set; } = "";

        public static readonly string[] StatusOptions =
        {
        "Plan to Take",
        "In Progress",
        "Completed",
        "Dropped"
    };
    }
}
