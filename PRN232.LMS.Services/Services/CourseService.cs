using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Services.Interfaces;

namespace PRN232.LMS.Services.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISemesterRepository _semesterRepository;

        public CourseService(ICourseRepository courseRepository, ISemesterRepository semesterRepository)
        {
            _courseRepository = courseRepository;
            _semesterRepository = semesterRepository;
        }

        public async Task<IEnumerable<CourseBusinessModel>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(MapToBusinessModel);
        }

        public async Task<CourseBusinessModel> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception($"Course with ID {courseId} not found.");
            return MapToBusinessModel(course);
        }

        public async Task<CourseBusinessModel> CreateCourseAsync(string courseName, int semesterId)
        {
            // Verify semester exists
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null)
                throw new Exception($"Semester with ID {semesterId} not found.");

            var course = new Course
            {
                CourseName = courseName,
                SemesterId = semesterId
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
            return MapToBusinessModel(course);
        }

        public async Task<CourseBusinessModel> UpdateCourseAsync(int courseId, string courseName, int semesterId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception($"Course with ID {courseId} not found.");

            // Verify semester exists
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null)
                throw new Exception($"Semester with ID {semesterId} not found.");

            course.CourseName = courseName;
            course.SemesterId = semesterId;

            _courseRepository.Update(course);
            await _courseRepository.SaveChangesAsync();
            return MapToBusinessModel(course);
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception($"Course with ID {courseId} not found.");

            _courseRepository.Delete(course);
            await _courseRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CourseBusinessModel>> GetCoursesBySemesterAsync(int semesterId)
        {
            var courses = await _courseRepository.GetCoursesBySemesterAsync(semesterId);
            return courses.Select(MapToBusinessModel);
        }

        public async Task<CourseBusinessModel> GetCourseWithEnrollmentsAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseWithEnrollmentsAsync(courseId);
            if (course == null)
                throw new Exception($"Course with ID {courseId} not found.");

            var model = MapToBusinessModel(course);
            if (course.Enrollments != null)
                model.EnrollmentIds = course.Enrollments.Select(e => e.EnrollmentId).ToList();

            return model;
        }

        private CourseBusinessModel MapToBusinessModel(Course course)
        {
            return new CourseBusinessModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                EnrollmentIds = course.Enrollments?.Select(e => e.EnrollmentId).ToList() ?? new()
            };
        }
    }
}
