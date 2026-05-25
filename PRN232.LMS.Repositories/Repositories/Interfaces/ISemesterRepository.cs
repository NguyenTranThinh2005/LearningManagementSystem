using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories.Interfaces
{
    public interface ISemesterRepository : IGenericRepository<Semester>
    {
        Task<Semester> GetSemesterWithCoursesAsync(int semesterId);
        Task<Semester> GetSemesterByNameAsync(string semesterName);
        Task<IEnumerable<Semester>> GetActiveSemestersAsync();
    }
}
