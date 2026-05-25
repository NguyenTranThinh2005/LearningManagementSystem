using Microsoft.AspNetCore.Mvc;
using PRN232.LAB1.SE193112.Models.Requests;
using PRN232.LAB1.SE193112.Models.Responses;
using PRN232.LMS.Services.Services.Interfaces;

namespace PRN232.LAB1.SE193112.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ILogger<SubjectsController> _logger;

        public SubjectsController(ISubjectService subjectService, ILogger<SubjectsController> logger)
        {
            _subjectService = subjectService;
            _logger = logger;
        }

        // GET: api/subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetAllSubjects()
        {
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                var responses = subjects.Select(s => new SubjectResponse
                {
                    SubjectId = s.SubjectId,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credit = s.Credit
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/subjects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectResponse>> GetSubjectById(int id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                var response = new SubjectResponse
                {
                    SubjectId = subject.SubjectId,
                    SubjectCode = subject.SubjectCode,
                    SubjectName = subject.SubjectName,
                    Credit = subject.Credit
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving subject {id}");
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/subjects
        [HttpPost]
        public async Task<ActionResult<SubjectResponse>> CreateSubject([FromBody] CreateSubjectRequest request)
        {
            try
            {
                var subject = await _subjectService.CreateSubjectAsync(
                    request.SubjectCode,
                    request.SubjectName,
                    request.Credit);

                var response = new SubjectResponse
                {
                    SubjectId = subject.SubjectId,
                    SubjectCode = subject.SubjectCode,
                    SubjectName = subject.SubjectName,
                    Credit = subject.Credit
                };

                return CreatedAtAction(nameof(GetSubjectById), new { id = subject.SubjectId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/subjects/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<SubjectResponse>> UpdateSubject(int id, [FromBody] UpdateSubjectRequest request)
        {
            try
            {
                if (id != request.SubjectId)
                    return BadRequest(new { message = "ID mismatch" });

                var subject = await _subjectService.UpdateSubjectAsync(
                    request.SubjectId,
                    request.SubjectCode,
                    request.SubjectName,
                    request.Credit);

                var response = new SubjectResponse
                {
                    SubjectId = subject.SubjectId,
                    SubjectCode = subject.SubjectCode,
                    SubjectName = subject.SubjectName,
                    Credit = subject.Credit
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating subject {id}");
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/subjects/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            try
            {
                var result = await _subjectService.DeleteSubjectAsync(id);
                if (result)
                    return NoContent();

                return NotFound(new { message = $"Subject with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subject {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/subjects/code/{code}
        [HttpGet("code/{code}")]
        public async Task<ActionResult<SubjectResponse>> GetSubjectByCode(string code)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByCodeAsync(code);
                var response = new SubjectResponse
                {
                    SubjectId = subject.SubjectId,
                    SubjectCode = subject.SubjectCode,
                    SubjectName = subject.SubjectName,
                    Credit = subject.Credit
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving subject with code {code}");
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/subjects/credit/{credit}
        [HttpGet("credit/{credit}")]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsByCredit(int credit)
        {
            try
            {
                var subjects = await _subjectService.GetSubjectsByCreditAsync(credit);
                var responses = subjects.Select(s => new SubjectResponse
                {
                    SubjectId = s.SubjectId,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credit = s.Credit
                }).ToList();

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving subjects with credit {credit}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
