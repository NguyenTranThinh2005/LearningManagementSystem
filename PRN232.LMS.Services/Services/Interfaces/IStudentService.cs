using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentBusinessModel>> GetAllStudentsAsync();
        Task<StudentBusinessModel> GetStudentByIdAsync(int studentId);
        Task<StudentBusinessModel> CreateStudentAsync(string fullName, string email, DateTime dateOfBirth);
        Task<StudentBusinessModel> UpdateStudentAsync(int studentId, string fullName, string email, DateTime dateOfBirth);
        Task<bool> DeleteStudentAsync(int studentId);
        Task<IEnumerable<StudentBusinessModel>> SearchStudentsByEmailAsync(string email);
        Task<StudentBusinessModel> GetStudentWithEnrollmentsAsync(int studentId);
        
        /// <summary>
        /// Get paged students with optional filtering, sorting, and expansion
        /// </summary>
        Task<(IEnumerable<StudentBusinessModel> Items, int TotalCount)> GetPagedStudentsAsync(
            int page,
            int pageSize,
            string search = "",
            string sortBy = "",
            bool ascending = true,
            string expand = "");
    }
}
