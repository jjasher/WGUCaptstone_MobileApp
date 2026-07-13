using SQLite;

namespace C971
{
    public class LocalDbService
    {
        private const string DB_NAME = "localdb.db3";
        private readonly SQLiteAsyncConnection _connection;

        private readonly Task _initTask;

        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _initTask = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await _connection.CreateTablesAsync(
                CreateFlags.None,
                typeof(Models.Term),
                typeof(Models.Course),
                typeof(Models.Assessment));

            await SeedAsync();
        }

        // only runs if the database is empty (first launch)
        private async Task SeedAsync()
        {
            var count = await _connection.Table<Models.Term>().CountAsync();
            if (count > 0) return;

            var term = new Models.Term
            {
                Title = "Term 1",
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 6, 30)
            };
            await _connection.InsertAsync(term);

            var course = new Models.Course
            {
                TermId = term.Id,
                Title = "Mobile Application Development Using C#",
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 3, 31),
                Status = "In Progress",
                InstructorName = "Anika Patel",
                Notes = "Sample course notes."
            };
            await _connection.InsertAsync(course);

            await SaveInstructorContactAsync(course.Id, "555-123-4567", "anika.patel@strimeuniversity.edu");

            await _connection.InsertAsync(new Models.Assessment
            {
                CourseId = course.Id,
                Name = "Mobile Dev Objective Assessment",
                Type = "Objective",
                StartDate = new DateTime(2025, 3, 15),
                DueDate = new DateTime(2025, 3, 31)
            });

            await _connection.InsertAsync(new Models.Assessment
            {
                CourseId = course.Id,
                Name = "Mobile Dev Performance Assessment",
                Type = "Performance",
                StartDate = new DateTime(2025, 3, 1),
                DueDate = new DateTime(2025, 3, 28)
            });
        }

        public async Task SaveInstructorContactAsync(int courseId, string phone, string email)
        {
            await SecureStorage.Default.SetAsync($"course_{courseId}_phone", phone);
            await SecureStorage.Default.SetAsync($"course_{courseId}_email", email);
        }

        public async Task<(string Phone, string Email)> GetInstructorContactAsync(int courseId)
        {
            var phone = await SecureStorage.Default.GetAsync($"course_{courseId}_phone") ?? "";
            var email = await SecureStorage.Default.GetAsync($"course_{courseId}_email") ?? "";
            return (phone, email);
        }

        public void RemoveInstructorContact(int courseId)
        {
            SecureStorage.Default.Remove($"course_{courseId}_phone");
            SecureStorage.Default.Remove($"course_{courseId}_email");
        }

        public async Task<List<Models.Term>> GetTermsAsync()
        {
            await _initTask;
            return await _connection.Table<Models.Term>().ToListAsync();
        }

        public async Task<Models.Term> GetTermAsync(int id)
        {
            await _initTask;
            return await _connection.Table<Models.Term>().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> SaveTermAsync(Models.Term term)
        {
            await _initTask;
            if (term.Id != 0)
                return await _connection.UpdateAsync(term);
            return await _connection.InsertAsync(term);
        }

        public async Task DeleteTermAsync(Models.Term term)
        {
            await _initTask;
            var courses = await GetCoursesForTermAsync(term.Id);
            foreach (var course in courses)
                await DeleteCourseAsync(course);
            await _connection.DeleteAsync(term);
        }

        public async Task<List<Models.Course>> GetCoursesForTermAsync(int termId)
        {
            await _initTask;
            return await _connection.Table<Models.Course>()
                .Where(c => c.TermId == termId)
                .ToListAsync();
        }

        public async Task<Models.Course> GetCourseAsync(int id)
        {
            await _initTask;
            return await _connection.Table<Models.Course>().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> SaveCourseAsync(Models.Course course)
        {
            await _initTask;
            if (course.Id != 0)
                return await _connection.UpdateAsync(course);
            return await _connection.InsertAsync(course);
        }

        public async Task DeleteCourseAsync(Models.Course course)
        {
            await _initTask;
            var assessments = await GetAssessmentsForCourseAsync(course.Id);
            foreach (var a in assessments)
                await _connection.DeleteAsync(a);
            RemoveInstructorContact(course.Id);
            await _connection.DeleteAsync(course);
        }

        public async Task<List<Models.Assessment>> GetAssessmentsForCourseAsync(int courseId)
        {
            await _initTask;
            return await _connection.Table<Models.Assessment>()
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Models.Assessment> GetAssessmentAsync(int id)
        {
            await _initTask;
            return await _connection.Table<Models.Assessment>().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> SaveAssessmentAsync(Models.Assessment assessment)
        {
            await _initTask;
            if (assessment.Id != 0)
                return await _connection.UpdateAsync(assessment);
            return await _connection.InsertAsync(assessment);
        }

        public async Task DeleteAssessmentAsync(Models.Assessment assessment)
        {
            await _initTask;
            await _connection.DeleteAsync(assessment);
        }

        public async Task<List<(Models.Term Term, Models.Course Course)>> GetCourseReportAsync()
        {
            await _initTask;
            var terms = await _connection.Table<Models.Term>().ToListAsync();
            var results = new List<(Models.Term, Models.Course)>();
            foreach (var term in terms)
            {
                var courses = await _connection.Table<Models.Course>()
                    .Where(c => c.TermId == term.Id)
                    .ToListAsync();
                foreach (var course in courses)
                    results.Add((term, course));
            }
            return results;
        }

        public async Task<List<Models.Course>> SearchCoursesAsync(string query)
        {
            await _initTask;
            var q = query.Trim().ToLower();
            var all = await _connection.Table<Models.Course>().ToListAsync();
            var results = new List<Models.Course>();
            foreach (var c in all)
            {
                if (c.Title.ToLower().Contains(q) ||
                    c.InstructorName.ToLower().Contains(q) ||
                    c.Status.ToLower().Contains(q) ||
                    c.Notes.ToLower().Contains(q))
                {
                    results.Add(c);
                }
            }
            return results;
        }
    }
}
