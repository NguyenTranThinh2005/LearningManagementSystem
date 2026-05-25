using Microsoft.AspNetCore.Mvc;
using PRN232.LAB1.SE193112.Models;
using PRN232.LAB1.SE193112.Models.Requests;
using PRN232.LAB1.SE193112.Models.Responses;
using PRN232.LMS.Services.Services.Interfaces;

namespace PRN232.LAB1.SE193112.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<CourseResponse>>>> GetAllCourses(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "",
            [FromQuery] bool ascending = true)
        {
            try
            {
                pageSize = pageSize > 100 ? 100 : pageSize;
                var courses = await _courseService.GetAllCoursesAsync();
                var responses = courses.Select(c => new CourseResponse
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId
                }).ToList();

                var totalCount = responses.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var pagedResponses = responses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                };

                var paginatedResponse = new PaginatedResponse<CourseResponse>
                {
                    Items = pagedResponses,
                    Pagination = pagination
                };

                return Ok(ApiResponse<PaginatedResponse<CourseResponse>>.Ok(
                    paginatedResponse,
                    "Courses retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
                return StatusCode(500, ApiResponse<PaginatedResponse<CourseResponse>>.InternalError(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourseById(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                var response = new CourseResponse
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    SemesterId = course.SemesterId
                };

                return Ok(ApiResponse<CourseResponse>.Ok(response, "Course retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving course {id}");
                return NotFound(ApiResponse<CourseResponse>.NotFound($"Course with ID {id} not found"));
            }
        }

        [HttpGet("{id}/enrollments")]
        public async Task<ActionResult<ApiResponse<CourseDetailResponse>>> GetCourseWithEnrollments(int id)
        {
            try
            {
                var course = await _courseService.GetCourseWithEnrollmentsAsync(id);
                var response = new CourseDetailResponse
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    SemesterId = course.SemesterId,
                    Enrollments = course.EnrollmentIds
                        .Select(eid => new EnrollmentSummaryResponse { EnrollmentId = eid })
                        .ToList()
                };

                return Ok(ApiResponse<CourseDetailResponse>.Ok(response, "Course with enrollments retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving course {id} with enrollments");
                return NotFound(ApiResponse<CourseDetailResponse>.NotFound($"Course with ID {id} not found"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseResponse>>> CreateCourse([FromBody] CreateCourseRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CourseName))
                    return BadRequest(ApiResponse<CourseResponse>.BadRequest("Course name is required"));

                var course = await _courseService.CreateCourseAsync(
                    request.CourseName,
                    request.SemesterId);

                var response = new CourseResponse
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    SemesterId = course.SemesterId
                };

                return CreatedAtAction(nameof(GetCourseById), 
                    new { id = course.CourseId },
                    ApiResponse<CourseResponse>.Created(response, "Course created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return StatusCode(500, ApiResponse<CourseResponse>.InternalError(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CourseResponse>>> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
        {
            try
            {
                if (id != request.CourseId)
                    return BadRequest(ApiResponse<CourseResponse>.BadRequest("ID mismatch"));

                if (string.IsNullOrWhiteSpace(request.CourseName))
                    return BadRequest(ApiResponse<CourseResponse>.BadRequest("Course name is required"));

                var course = await _courseService.UpdateCourseAsync(
                    request.CourseId,
                    request.CourseName,
                    request.SemesterId);

                var response = new CourseResponse
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    SemesterId = course.SemesterId
                };

                return Ok(ApiResponse<CourseResponse>.Ok(response, "Course updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating course {id}");
                return NotFound(ApiResponse<CourseResponse>.NotFound($"Course with ID {id} not found"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCourse(int id)
        {
            try
            {
                var result = await _courseService.DeleteCourseAsync(id);
                if (result)
                    return NoContent();

                return NotFound(ApiResponse<object>.NotFound($"Course with ID {id} not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting course {id}");
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpGet("semester/{semesterId}")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<CourseResponse>>>> GetCoursesBySemester(
            int semesterId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                pageSize = pageSize > 100 ? 100 : pageSize;
                var courses = await _courseService.GetCoursesBySemesterAsync(semesterId);
                var responses = courses.Select(c => new CourseResponse
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId
                }).ToList();

                var totalCount = responses.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var pagedResponses = responses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                };

                var paginatedResponse = new PaginatedResponse<CourseResponse>
                {
                    Items = pagedResponses,
                    Pagination = pagination
                };

                return Ok(ApiResponse<PaginatedResponse<CourseResponse>>.Ok(
                    paginatedResponse,
                    "Courses retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving courses for semester {semesterId}");
                return StatusCode(500, ApiResponse<PaginatedResponse<CourseResponse>>.InternalError(ex.Message));
            }
        }
    }
}
