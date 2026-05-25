using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly Prn232LmsContext _context;

        public CourseRepository(Prn232LmsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Course> GetCourseWithEnrollmentsAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<IEnumerable<Course>> GetCoursesBySemesterAsync(int semesterId)
        {
            return await _context.Courses
                .Where(c => c.SemesterId == semesterId)
                .Include(c => c.Enrollments)
                .ToListAsync();
        }

        public async Task<Course> GetCourseWithSemesterAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Semester)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }
    }
}
