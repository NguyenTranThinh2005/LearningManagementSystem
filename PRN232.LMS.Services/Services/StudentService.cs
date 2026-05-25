using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Services.Interfaces;
using System.Linq.Expressions;

namespace PRN232.LMS.Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<StudentBusinessModel>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToBusinessModel);
        }

        public async Task<StudentBusinessModel> GetStudentByIdAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception($"Student with ID {studentId} not found.");
            return MapToBusinessModel(student);
        }

        public async Task<StudentBusinessModel> CreateStudentAsync(string fullName, string email, DateTime dateOfBirth)
        {
            var student = new Student
            {
                FullName = fullName,
                Email = email,
                DateOfBirth = dateOfBirth
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();
            return MapToBusinessModel(student);
        }

        public async Task<StudentBusinessModel> UpdateStudentAsync(int studentId, string fullName, string email, DateTime dateOfBirth)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception($"Student with ID {studentId} not found.");

            student.FullName = fullName;
            student.Email = email;
            student.DateOfBirth = dateOfBirth;

            _studentRepository.Update(student);
            await _studentRepository.SaveChangesAsync();
            return MapToBusinessModel(student);
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception($"Student with ID {studentId} not found.");

            _studentRepository.Delete(student);
            await _studentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentBusinessModel>> SearchStudentsByEmailAsync(string email)
        {
            var students = await _studentRepository.GetStudentsByEmailAsync(email);
            return students.Select(MapToBusinessModel);
        }

        public async Task<StudentBusinessModel> GetStudentWithEnrollmentsAsync(int studentId)
        {
            var student = await _studentRepository.GetStudentWithEnrollmentsAsync(studentId);
            if (student == null)
                throw new Exception($"Student with ID {studentId} not found.");

            return MapToBusinessModel(student);
        }

        public async Task<(IEnumerable<StudentBusinessModel> Items, int TotalCount)> GetPagedStudentsAsync(
            int page,
            int pageSize,
            string search = "",
            string sortBy = "",
            bool ascending = true,
            string expand = "")
        {
            Expression<Func<Student, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter = s => s.FullName.Contains(search) || s.Email.Contains(search);
            }

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortBy = "StudentId";
            }

            string[]? includes = string.IsNullOrWhiteSpace(expand) 
                ? null 
                : expand.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var (students, totalCount) = await _studentRepository.GetPagedAsync(
                page,
                pageSize,
                filter,
                sortBy,
                ascending,
                includes);

            return (students.Select(MapToBusinessModel), totalCount);
        }

        private StudentBusinessModel MapToBusinessModel(Student student)
        {
            return new StudentBusinessModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                EnrollmentIds = student.Enrollments?.Select(e => e.EnrollmentId).ToList() ?? new(),
                Enrollments = student.Enrollments?.Select(e => new EnrollmentBusinessModel
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    Course = e.Course == null ? null : new CourseBusinessModel
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        SemesterId = e.Course.SemesterId
                    }
                }).ToList() ?? new()
            };
        }
    }
}
