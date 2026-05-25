using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course> GetCourseWithEnrollmentsAsync(int courseId);
        Task<IEnumerable<Course>> GetCoursesBySemesterAsync(int semesterId);
        Task<Course> GetCourseWithSemesterAsync(int courseId);
    }
}
