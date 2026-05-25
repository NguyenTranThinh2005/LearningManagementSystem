using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Services.Interfaces;

namespace PRN232.LMS.Services.Services
{
    public class SemesterServiceImplementation : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;

        public SemesterServiceImplementation(ISemesterRepository semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<IEnumerable<SemesterBusinessModel>> GetAllSemestersAsync()
        {
            var semesters = await _semesterRepository.GetAllAsync();
            return semesters.Select(MapToBusinessModel);
        }

        public async Task<SemesterBusinessModel> GetSemesterByIdAsync(int semesterId)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null)
                throw new Exception($"Semester with ID {semesterId} not found.");
            return MapToBusinessModel(semester);
        }

        public async Task<SemesterBusinessModel> CreateSemesterAsync(string semesterName, DateTime startDate, DateTime endDate)
        {
            var semester = new Semester
            {
                SemesterName = semesterName,
                StartDate = startDate,
                EndDate = endDate
            };

            await _semesterRepository.AddAsync(semester);
            await _semesterRepository.SaveChangesAsync();
            return MapToBusinessModel(semester);
        }

        public async Task<SemesterBusinessModel> UpdateSemesterAsync(int semesterId, string semesterName, DateTime startDate, DateTime endDate)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null)
                throw new Exception($"Semester with ID {semesterId} not found.");

            semester.SemesterName = semesterName;
            semester.StartDate = startDate;
            semester.EndDate = endDate;

            _semesterRepository.Update(semester);
            await _semesterRepository.SaveChangesAsync();
            return MapToBusinessModel(semester);
        }

        public async Task<bool> DeleteSemesterAsync(int semesterId)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null)
                throw new Exception($"Semester with ID {semesterId} not found.");

            _semesterRepository.Delete(semester);
            await _semesterRepository.SaveChangesAsync();
            return true;
        }

        public async Task<SemesterBusinessModel> GetSemesterWithCoursesAsync(int semesterId)
        {
            var semester = await _semesterRepository.GetSemesterWithCoursesAsync(semesterId);
            if (semester == null)
                throw new Exception($"Semester with ID {semesterId} not found.");

            var model = MapToBusinessModel(semester);
            if (semester.Courses != null)
                model.CourseIds = semester.Courses.Select(c => c.CourseId).ToList();

            return model;
        }

        public async Task<IEnumerable<SemesterBusinessModel>> GetActiveSemestersAsync()
        {
            var semesters = await _semesterRepository.GetActiveSemestersAsync();
            return semesters.Select(MapToBusinessModel);
        }

        private SemesterBusinessModel MapToBusinessModel(Semester semester)
        {
            var now = DateTime.Now;
            return new SemesterBusinessModel
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                CourseIds = semester.Courses?.Select(c => c.CourseId).ToList() ?? new(),
                IsActive = semester.StartDate <= now && semester.EndDate >= now
            };
        }
    }
}
