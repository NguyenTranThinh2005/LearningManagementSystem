using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories.Interfaces
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        Task<Subject> GetSubjectByCodeAsync(string subjectCode);
        Task<IEnumerable<Subject>> GetSubjectsByCreditAsync(int credit);
    }
}
