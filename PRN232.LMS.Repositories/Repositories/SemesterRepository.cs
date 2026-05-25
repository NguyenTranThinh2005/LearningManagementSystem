using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories
{
    public class SemesterRepository : GenericRepository<Semester>, ISemesterRepository
    {
        private readonly Prn232LmsContext _context;

        public SemesterRepository(Prn232LmsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Semester> GetSemesterWithCoursesAsync(int semesterId)
        {
            return await _context.Semesters
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.SemesterId == semesterId);
        }

        public async Task<Semester> GetSemesterByNameAsync(string semesterName)
        {
            return await _context.Semesters
                .FirstOrDefaultAsync(s => s.SemesterName == semesterName);
        }

        public async Task<IEnumerable<Semester>> GetActiveSemestersAsync()
        {
            var now = DateTime.Now;
            return await _context.Semesters
                .Where(s => s.StartDate <= now && s.EndDate >= now)
                .Include(s => s.Courses)
                .ToListAsync();
        }
    }
}
