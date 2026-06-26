using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Models
{
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int CourseId { get; set; }

        [NotNull]
        public string Name { get; set; } = "";

        public string Type { get; set; } = "Objective";

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime DueDate { get; set; } = DateTime.Today.AddMonths(3);

        public static readonly string[] TypeOptions = { "Objective", "Performance" };
    }
}
