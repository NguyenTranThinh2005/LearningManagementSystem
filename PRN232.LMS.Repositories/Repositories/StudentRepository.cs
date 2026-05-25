using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        private readonly Prn232LmsContext _context;

        public StudentRepository(Prn232LmsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Student> GetStudentWithEnrollmentsAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<IEnumerable<Student>> GetStudentsByEmailAsync(string email)
        {
            return await _context.Students
                .Where(s => s.Email.Contains(email))
                .ToListAsync();
        }
    }
}
