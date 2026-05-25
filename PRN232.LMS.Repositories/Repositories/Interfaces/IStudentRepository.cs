using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student> GetStudentWithEnrollmentsAsync(int studentId);
        Task<IEnumerable<Student>> GetStudentsByEmailAsync(string email);
    }
}
