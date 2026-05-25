using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<IEnumerable<SemesterBusinessModel>> GetAllSemestersAsync();
        Task<SemesterBusinessModel> GetSemesterByIdAsync(int semesterId);
        Task<SemesterBusinessModel> CreateSemesterAsync(string semesterName, DateTime startDate, DateTime endDate);
        Task<SemesterBusinessModel> UpdateSemesterAsync(int semesterId, string semesterName, DateTime startDate, DateTime endDate);
        Task<bool> DeleteSemesterAsync(int semesterId);
        Task<SemesterBusinessModel> GetSemesterWithCoursesAsync(int semesterId);
        Task<IEnumerable<SemesterBusinessModel>> GetActiveSemestersAsync();
    }
}
