using Microsoft.AspNetCore.Mvc;
using PRN232.LAB1.SE193112.Models.Requests;
using PRN232.LAB1.SE193112.Models.Responses;
using PRN232.LMS.Services.Services.Interfaces;

namespace PRN232.LAB1.SE193112.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        private readonly ILogger<SemestersController> _logger;

        public SemestersController(ISemesterService semesterService, ILogger<SemestersController> logger)
        {
            _semesterService = semesterService;
            _logger = logger;
        }

        // GET: api/semesters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SemesterResponse>>> GetAllSemesters()
        {
            try
            {
                var semesters = await _semesterService.GetAllSemestersAsync();
                var responses = semesters.Select(s => new SemesterResponse
                {
                    SemesterId = s.SemesterId,
                    SemesterName = s.SemesterName,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving semesters");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/semesters/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SemesterResponse>> GetSemesterById(int id)
        {
            try
            {
                var semester = await _semesterService.GetSemesterByIdAsync(id);
                var response = new SemesterResponse
                {
                    SemesterId = semester.SemesterId,
                    SemesterName = semester.SemesterName,
                    StartDate = semester.StartDate,
                    EndDate = semester.EndDate
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving semester {id}");
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/semesters/{id}/courses
        [HttpGet("{id}/courses")]
        public async Task<ActionResult<SemesterDetailResponse>> GetSemesterWithCourses(int id)
        {
            try
            {
                var semester = await _semesterService.GetSemesterWithCoursesAsync(id);
                var response = new SemesterDetailResponse
                {
                    SemesterId = semester.SemesterId,
                    SemesterName = semester.SemesterName,
                    StartDate = semester.StartDate,
                    EndDate = semester.EndDate,
                    Courses = semester.CourseIds
                        .Select(cid => new CourseResponse { CourseId = cid })
                        .ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving semester {id} with courses");
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/semesters
        [HttpPost]
        public async Task<ActionResult<SemesterResponse>> CreateSemester([FromBody] CreateSemesterRequest request)
        {
            try
            {
                var semester = await _semesterService.CreateSemesterAsync(
                    request.SemesterName,
                    request.StartDate,
                    request.EndDate);

                var response = new SemesterResponse
                {
                    SemesterId = semester.SemesterId,
                    SemesterName = semester.SemesterName,
                    StartDate = semester.StartDate,
                    EndDate = semester.EndDate
                };

                return CreatedAtAction(nameof(GetSemesterById), new { id = semester.SemesterId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating semester");
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/semesters/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<SemesterResponse>> UpdateSemester(int id, [FromBody] UpdateSemesterRequest request)
        {
            try
            {
                if (id != request.SemesterId)
                    return BadRequest(new { message = "ID mismatch" });

                var semester = await _semesterService.UpdateSemesterAsync(
                    request.SemesterId,
                    request.SemesterName,
                    request.StartDate,
                    request.EndDate);

                var response = new SemesterResponse
                {
                    SemesterId = semester.SemesterId,
                    SemesterName = semester.SemesterName,
                    StartDate = semester.StartDate,
                    EndDate = semester.EndDate
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating semester {id}");
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/semesters/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSemester(int id)
        {
            try
            {
                var result = await _semesterService.DeleteSemesterAsync(id);
                if (result)
                    return NoContent();

                return NotFound(new { message = $"Semester with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting semester {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/semesters/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SemesterResponse>>> GetActiveSemesters()
        {
            try
            {
                var semesters = await _semesterService.GetActiveSemestersAsync();
                var responses = semesters.Select(s => new SemesterResponse
                {
                    SemesterId = s.SemesterId,
                    SemesterName = s.SemesterName,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active semesters");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
