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
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all students with pagination, filtering, sorting, and expansion
        /// </summary>
        /// <remarks>
        /// Query Parameters:
        /// - page: Page number (default: 1)
        /// - pageSize: Items per page (default: 10, max: 100)
        /// - search: Search by full name or email
        /// - sortBy: Field to sort by (e.g., StudentId, FullName, Email, DateOfBirth)
        /// - ascending: Sort order true/false (default: true)
        /// - expand: Include related data (e.g., enrollments)
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<object>>>> GetAllStudents(
            [FromQuery] QueryParameters parameters)
        {
            try
            {
                var (students, totalCount) = await _studentService.GetPagedStudentsAsync(
                    parameters.Page, 
                    parameters.PageSize, 
                    parameters.Search, 
                    parameters.SortBy, 
                    parameters.Ascending,
                    parameters.Expand);

                var studentResponses = students.Select(s => new StudentResponse
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth
                }).ToList();

                // Apply selection if fields are specified
                IEnumerable<object> finalItems = studentResponses;
                if (!string.IsNullOrWhiteSpace(parameters.Fields))
                {
                    finalItems = SelectionHelper.SelectFields(studentResponses, parameters.Fields);
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
                    "Students retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving students");
                return StatusCode(500, ApiResponse<PaginatedResponse<object>>.InternalError(ex.Message));
            }
        }

        /// <summary>
        /// Get student by ID with complete related data
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentDetailResponse>>> GetStudentById(int id)
        {
            try
            {
                var student = await _studentService.GetStudentWithEnrollmentsAsync(id);
                var response = new StudentDetailResponse
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth,
                    Enrollments = student.Enrollments
                        .Select(e => new EnrollmentSummaryResponse 
                        { 
                            EnrollmentId = e.EnrollmentId,
                            StudentId = e.StudentId,
                            CourseId = e.CourseId,
                            EnrollDate = e.EnrollDate,
                            Status = e.Status
                        })
                        .ToList()
                };

                return Ok(ApiResponse<StudentDetailResponse>.Ok(response, "Student retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving student {id}");
                return NotFound(ApiResponse<StudentDetailResponse>.NotFound($"Student with ID {id} not found"));
            }
        }

        /// <summary>
        /// Get student with enrollments
        /// </summary>
        [HttpGet("{id}/enrollments")]
        public async Task<ActionResult<ApiResponse<StudentDetailResponse>>> GetStudentWithEnrollments(int id)
        {
            try
            {
                var student = await _studentService.GetStudentWithEnrollmentsAsync(id);
                var response = new StudentDetailResponse
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth,
                    Enrollments = student.EnrollmentIds
                        .Select(eid => new EnrollmentSummaryResponse { EnrollmentId = eid })
                        .ToList()
                };

                return Ok(ApiResponse<StudentDetailResponse>.Ok(response, "Student with enrollments retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving student {id} with enrollments");
                return NotFound(ApiResponse<StudentDetailResponse>.NotFound($"Student with ID {id} not found"));
            }
        }

        /// <summary>
        /// Create a new student
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentResponse>>> CreateStudent([FromBody] CreateStudentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FullName))
                    return BadRequest(ApiResponse<StudentResponse>.BadRequest("Full name is required"));

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(ApiResponse<StudentResponse>.BadRequest("Email is required"));

                var student = await _studentService.CreateStudentAsync(
                    request.FullName,
                    request.Email,
                    request.DateOfBirth);

                var response = new StudentResponse
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth
                };

                return CreatedAtAction(nameof(GetStudentById), 
                    new { id = student.StudentId },
                    ApiResponse<StudentResponse>.Created(response, "Student created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                return StatusCode(500, ApiResponse<StudentResponse>.InternalError(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing student
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<StudentResponse>>> UpdateStudent(int id, [FromBody] UpdateStudentRequest request)
        {
            try
            {
                if (id != request.StudentId)
                    return BadRequest(ApiResponse<StudentResponse>.BadRequest("ID mismatch"));

                if (string.IsNullOrWhiteSpace(request.FullName))
                    return BadRequest(ApiResponse<StudentResponse>.BadRequest("Full name is required"));

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(ApiResponse<StudentResponse>.BadRequest("Email is required"));

                var student = await _studentService.UpdateStudentAsync(
                    request.StudentId,
                    request.FullName,
                    request.Email,
                    request.DateOfBirth);

                var response = new StudentResponse
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth
                };

                return Ok(ApiResponse<StudentResponse>.Ok(response, "Student updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating student {id}");
                return NotFound(ApiResponse<StudentResponse>.NotFound($"Student with ID {id} not found"));
            }
        }

        /// <summary>
        /// Delete a student
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStudent(int id)
        {
            try
            {
                var result = await _studentService.DeleteStudentAsync(id);
                if (result)
                    return NoContent();

                return NotFound(ApiResponse<object>.NotFound($"Student with ID {id} not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting student {id}");
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        /// <summary>
        /// Search students by email with pagination
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<StudentResponse>>>> SearchByEmail(
            [FromQuery] string email,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(ApiResponse<PaginatedResponse<StudentResponse>>.BadRequest("Email search term is required"));

                pageSize = pageSize > 100 ? 100 : pageSize;
                var students = await _studentService.SearchStudentsByEmailAsync(email);
                // Since the service only returns IEnumerable<StudentBusinessModel>, we need to do pagination manually:
                var pagedStudents = students.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var totalCount = students.Count();

                var studentResponses = students.Select(s => new StudentResponse
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth
                }).ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                };

                var paginatedResponse = new PaginatedResponse<StudentResponse>
                {
                    Items = studentResponses,
                    Pagination = pagination
                };

                return Ok(ApiResponse<PaginatedResponse<StudentResponse>>.Ok(
                    paginatedResponse,
                    "Students found successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching students");
                return StatusCode(500, ApiResponse<PaginatedResponse<StudentResponse>>.InternalError(ex.Message));
            }
        }
    }
}
