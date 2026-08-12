using EduAssign.API.DTOs.Submissions;
using EduAssign.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduAssign.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionsController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        // POST: /api/submissions
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] CreateSubmissionDto dto)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (studentId == null) return Unauthorized();

            try
            {
                var submission = await _submissionService.CreateSubmissionAsync(studentId, dto);
                if (submission == null) return NotFound("Assignment not found.");
                return Ok(submission);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: /api/submissions/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubmissions()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (studentId == null) return Unauthorized();

            var submissions = await _submissionService.GetMySubmissionsAsync(studentId);
            return Ok(submissions);
        }

        // GET: /api/submissions/assignment/{assignmentId} (For student to check their specific submission)
        [HttpGet("assignment/{assignmentId}")]
        public async Task<IActionResult> GetMySubmissionForAssignment(string assignmentId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (studentId == null) return Unauthorized();

            var submission = await _submissionService.GetSubmissionByAssignmentAsync(studentId, assignmentId);
            return Ok(submission); // Returns 204 No Content if null, which is fine.
        }

        // PUT: /api/submissions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateSubmissionDto dto)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (studentId == null) return Unauthorized();

            try
            {
                var result = await _submissionService.UpdateSubmissionAsync(studentId, id, dto);
                if (result == null) return NotFound("Submission not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: /api/submissions/{id}/grade (For Teacher)
        [HttpPut("{id}/grade")]
        public async Task<IActionResult> Grade(string id, [FromBody] GradeSubmissionDto dto)
        {
            // Note: In a real app, verify the user has the 'teacher' role here.
            var result = await _submissionService.GradeSubmissionAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}