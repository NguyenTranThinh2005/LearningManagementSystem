using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        private readonly Prn232LmsContext _context;

        public SubjectRepository(Prn232LmsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Subject> GetSubjectByCodeAsync(string subjectCode)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode);
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByCreditAsync(int credit)
        {
            return await _context.Subjects
                .Where(s => s.Credit == credit)
                .ToListAsync();
        }
    }
}
