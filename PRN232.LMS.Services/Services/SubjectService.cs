using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Services.Interfaces;

namespace PRN232.LMS.Services.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<IEnumerable<SubjectBusinessModel>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetAllAsync();
            return subjects.Select(MapToBusinessModel);
        }

        public async Task<SubjectBusinessModel> GetSubjectByIdAsync(int subjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null)
                throw new Exception($"Subject with ID {subjectId} not found.");
            return MapToBusinessModel(subject);
        }

        public async Task<SubjectBusinessModel> CreateSubjectAsync(string subjectCode, string subjectName, int credit)
        {
            var subject = new Subject
            {
                SubjectCode = subjectCode,
                SubjectName = subjectName,
                Credit = credit
            };

            await _subjectRepository.AddAsync(subject);
            await _subjectRepository.SaveChangesAsync();
            return MapToBusinessModel(subject);
        }

        public async Task<SubjectBusinessModel> UpdateSubjectAsync(int subjectId, string subjectCode, string subjectName, int credit)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null)
                throw new Exception($"Subject with ID {subjectId} not found.");

            subject.SubjectCode = subjectCode;
            subject.SubjectName = subjectName;
            subject.Credit = credit;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveChangesAsync();
            return MapToBusinessModel(subject);
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null)
                throw new Exception($"Subject with ID {subjectId} not found.");

            _subjectRepository.Delete(subject);
            await _subjectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<SubjectBusinessModel> GetSubjectByCodeAsync(string subjectCode)
        {
            var subject = await _subjectRepository.GetSubjectByCodeAsync(subjectCode);
            if (subject == null)
                throw new Exception($"Subject with code {subjectCode} not found.");
            return MapToBusinessModel(subject);
        }

        public async Task<IEnumerable<SubjectBusinessModel>> GetSubjectsByCreditAsync(int credit)
        {
            var subjects = await _subjectRepository.GetSubjectsByCreditAsync(credit);
            return subjects.Select(MapToBusinessModel);
        }

        private SubjectBusinessModel MapToBusinessModel(Subject subject)
        {
            return new SubjectBusinessModel
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };
        }
    }
}
