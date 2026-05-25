using Microsoft.AspNetCore.Mvc;
using PRN232.LAB1.SE193112.Models;
using PRN232.LAB1.SE193112.Models.Requests;
using PRN232.LAB1.SE193112.Models.Responses;
using PRN232.LMS.Services.Services.Interfaces;
using PRN232.LAB1.SE193112.Helpers;

namespace PRN232.LAB1.SE193112.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(IEnrollmentService enrollmentService, ILogger<EnrollmentsController> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        // GET: api/enrollments
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<object>>>> GetAllEnrollments(
            [FromQuery] QueryParameters parameters)
        {
            try
            {
                var (enrollments, totalCount) = await _enrollmentService.GetPagedEnrollmentsAsync(
                    parameters.Page,
                    parameters.PageSize,
                    parameters.Search,
                    parameters.SortBy,
                    parameters.Ascending,
                    parameters.Expand);

                var responses = enrollments.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    Student = e.Student == null ? null : new StudentResponse
                    {
                        StudentId = e.Student.StudentId,
                        FullName = e.Student.FullName,
                        Email = e.Student.Email,
                        DateOfBirth = e.Student.DateOfBirth
                    },
                    Course = e.Course == null ? null : new CourseResponse
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        SemesterId = e.Course.SemesterId
                    }
                }).ToList();

                // Apply selection if fields are specified
                IEnumerable<object> finalItems = responses;
                if (!string.IsNullOrWhiteSpace(parameters.Fields))
                {
                    finalItems = SelectionHelper.SelectFields(responses, parameters.Fields);
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);
                var pagination = new PaginationMetadata
                {
                    Page = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                };

                var paginatedResponse = new PaginatedResponse<object>
                {
                    Items = finalItems.ToList(),
                    Pagination = pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(
                    paginatedResponse,
                    "Enrollments retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments");
                return StatusCode(500, ApiResponse<PaginatedResponse<object>>.InternalError(ex.Message));
            }
        }

        // GET: api/enrollments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EnrollmentDetailResponse>>> GetEnrollmentById(int id)
        {
            try
            {
                var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
                var response = new EnrollmentDetailResponse
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    EnrollDate = enrollment.EnrollDate,
                    Status = enrollment.Status,
                    Student = enrollment.Student == null ? null : new StudentResponse
                    {
                        StudentId = enrollment.Student.StudentId,
                        FullName = enrollment.Student.FullName,
                        Email = enrollment.Student.Email,
                        DateOfBirth = enrollment.Student.DateOfBirth
                    },
                    Course = enrollment.Course == null ? null : new CourseResponse
                    {
                        CourseId = enrollment.Course.CourseId,
                        CourseName = enrollment.Course.CourseName,
                        SemesterId = enrollment.Course.SemesterId
                    }
                };

                return Ok(ApiResponse<EnrollmentDetailResponse>.Ok(response, "Enrollment retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving enrollment {id}");
                return NotFound(ApiResponse<EnrollmentDetailResponse>.NotFound($"Enrollment with ID {id} not found"));
            }
        }

        // POST: api/enrollments
        [HttpPost]
        public async Task<ActionResult<EnrollmentResponse>> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            try
            {
                var enrollment = await _enrollmentService.CreateEnrollmentAsync(
                    request.StudentId,
                    request.CourseId,
                    request.EnrollDate,
                    request.Status);

                var response = new EnrollmentResponse
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId
                };

                return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.EnrollmentId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating enrollment");
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/enrollments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<EnrollmentResponse>> UpdateEnrollment(int id, [FromBody] UpdateEnrollmentRequest request)
        {
            try
            {
                if (id != request.EnrollmentId)
                    return BadRequest(new { message = "ID mismatch" });

                var enrollment = await _enrollmentService.UpdateEnrollmentAsync(
                    request.EnrollmentId,
                    request.StudentId,
                    request.CourseId,
                    request.EnrollDate,
                    request.Status);

                var response = new EnrollmentResponse
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating enrollment {id}");
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/enrollments/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEnrollment(int id)
        {
            try
            {
                var result = await _enrollmentService.DeleteEnrollmentAsync(id);
                if (result)
                    return NoContent();

                return NotFound(new { message = $"Enrollment with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting enrollment {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/enrollments/student/{studentId}
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentResponse>>> GetEnrollmentsByStudent(int studentId)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(studentId);
                var responses = enrollments.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving enrollments for student {studentId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/enrollments/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentResponse>>> GetEnrollmentsByCourse(int courseId)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
                var responses = enrollments.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving enrollments for course {courseId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/enrollments/status/{status}
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<EnrollmentResponse>>> GetEnrollmentsByStatus(string status)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByStatusAsync(status);
                var responses = enrollments.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving enrollments with status {status}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
