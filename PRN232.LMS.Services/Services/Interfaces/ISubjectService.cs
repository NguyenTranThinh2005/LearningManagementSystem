using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectBusinessModel>> GetAllSubjectsAsync();
        Task<SubjectBusinessModel> GetSubjectByIdAsync(int subjectId);
        Task<SubjectBusinessModel> CreateSubjectAsync(string subjectCode, string subjectName, int credit);
        Task<SubjectBusinessModel> UpdateSubjectAsync(int subjectId, string subjectCode, string subjectName, int credit);
        Task<bool> DeleteSubjectAsync(int subjectId);
        Task<SubjectBusinessModel> GetSubjectByCodeAsync(string subjectCode);
        Task<IEnumerable<SubjectBusinessModel>> GetSubjectsByCreditAsync(int credit);
    }
}
