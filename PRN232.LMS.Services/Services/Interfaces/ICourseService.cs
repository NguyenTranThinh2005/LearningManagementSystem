using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseBusinessModel>> GetAllCoursesAsync();
        Task<CourseBusinessModel> GetCourseByIdAsync(int courseId);
        Task<CourseBusinessModel> CreateCourseAsync(string courseName, int semesterId);
        Task<CourseBusinessModel> UpdateCourseAsync(int courseId, string courseName, int semesterId);
        Task<bool> DeleteCourseAsync(int courseId);
        Task<IEnumerable<CourseBusinessModel>> GetCoursesBySemesterAsync(int semesterId);
        Task<CourseBusinessModel> GetCourseWithEnrollmentsAsync(int courseId);
    }
}
