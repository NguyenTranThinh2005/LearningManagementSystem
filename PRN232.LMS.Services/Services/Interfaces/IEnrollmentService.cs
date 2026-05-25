using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentBusinessModel>> GetAllEnrollmentsAsync();
        Task<EnrollmentBusinessModel> GetEnrollmentByIdAsync(int enrollmentId);
        Task<EnrollmentBusinessModel> CreateEnrollmentAsync(int studentId, int courseId, DateTime enrollDate, string status);
        Task<EnrollmentBusinessModel> UpdateEnrollmentAsync(int enrollmentId, int studentId, int courseId, DateTime enrollDate, string status);
        Task<bool> DeleteEnrollmentAsync(int enrollmentId);
        Task<IEnumerable<EnrollmentBusinessModel>> GetEnrollmentsByStudentAsync(int studentId);
        Task<IEnumerable<EnrollmentBusinessModel>> GetEnrollmentsByCourseAsync(int courseId);
        Task<IEnumerable<EnrollmentBusinessModel>> GetEnrollmentsByStatusAsync(string status);
        
        Task<(IEnumerable<EnrollmentBusinessModel> Items, int TotalCount)> GetPagedEnrollmentsAsync(
            int page,
            int pageSize,
            string search = "",
            string sortBy = "",
            bool ascending = true,
            string expand = "");
    }
}
