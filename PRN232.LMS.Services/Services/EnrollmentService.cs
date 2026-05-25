using System.Linq.Expressions;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Services.Interfaces;

namespace PRN232.LMS.Services.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<EnrollmentBusinessModel>> GetAllEnrollmentsAsync()
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            return enrollments.Select(MapToBusinessModel);
        }

        public async Task<EnrollmentBusinessModel> GetEnrollmentByIdAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentWithDetailsAsync(enrollmentId);
            if (enrollment == null)
                throw new Exception($"Enrollment with ID {enrollmentId} not found.");
            return MapToBusinessModel(enrollment);
        }

        public async Task<EnrollmentBusinessModel> CreateEnrollmentAsync(int studentId, int courseId, DateTime enrollDate, string status)
        {
            // Verify student exists
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception($"Student with ID {studentId} not found.");

            // Verify course exists
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception($"Course with ID {courseId} not found.");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollDate = enrollDate,
                Status = status
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return MapToBusinessModel(enrollment);
        }

        public async Task<EnrollmentBusinessModel> UpdateEnrollmentAsync(int enrollmentId, int studentId, int courseId, DateTime enrollDate, string status)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                throw new Exception($"Enrollment with ID {enrollmentId} not found.");

            // Verify student exists
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception($"Student with ID {studentId} not found.");

            // Verify course exists
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception($"Course with ID {courseId} not found.");

            enrollment.StudentId = studentId;
            enrollment.CourseId = courseId;
            enrollment.EnrollDate = enrollDate;
            enrollment.Status = status;

            _enrollmentRepository.Update(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return MapToBusinessModel(enrollment);
        }

        public async Task<bool> DeleteEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                throw new Exception($"Enrollment with ID {enrollmentId} not found.");

            _enrollmentRepository.Delete(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EnrollmentBusinessModel>> GetEnrollmentsByStudentAsync(int studentId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentAsync(studentId);
            return enrollments.Select(MapToBusinessModel);
        }

        public async Task<IEnumerable<EnrollmentBusinessModel>> GetEnrollmentsByCourseAsync(int courseId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
            return enrollments.Select(MapToBusinessModel);
        }

        public async Task<IEnumerable<EnrollmentBusinessModel>> GetEnrollmentsByStatusAsync(string status)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStatusAsync(status);
            return enrollments.Select(MapToBusinessModel);
        }

        public async Task<(IEnumerable<EnrollmentBusinessModel> Items, int TotalCount)> GetPagedEnrollmentsAsync(
            int page,
            int pageSize,
            string search = "",
            string sortBy = "",
            bool ascending = true,
            string expand = "")
        {
            Expression<Func<Enrollment, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter = e => e.Status.Contains(search);
            }

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortBy = "EnrollmentId";
            }

            string[]? includes = string.IsNullOrWhiteSpace(expand) 
                ? null 
                : expand.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var (enrollments, totalCount) = await _enrollmentRepository.GetPagedAsync(
                page,
                pageSize,
                filter,
                sortBy,
                ascending,
                includes);

            return (enrollments.Select(MapToBusinessModel), totalCount);
        }

        private EnrollmentBusinessModel MapToBusinessModel(Enrollment enrollment)
        {
            var model = new EnrollmentBusinessModel
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status
            };

            if (enrollment.Student != null)
            {
                model.Student = new StudentBusinessModel
                {
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Email = enrollment.Student.Email,
                    DateOfBirth = enrollment.Student.DateOfBirth
                };
            }

            if (enrollment.Course != null)
            {
                model.Course = new CourseBusinessModel
                {
                    CourseId = enrollment.Course.CourseId,
                    CourseName = enrollment.Course.CourseName,
                    SemesterId = enrollment.Course.SemesterId
                };
            }

            return model;
        }
    }
}
