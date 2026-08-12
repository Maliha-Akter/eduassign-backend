using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduAssign.API.DTOs.Submissions;
using EduAssign.API.Services;
using System.Security.Claims;

namespace EduAssign.API.Controllers;

[ApiController]
[Route("api/submissions")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    private string GetUserId() => 
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;

    [HttpPost]
    public async Task<IActionResult> CreateSubmission([FromBody] CreateSubmissionDto dto)
    {
        var studentId = GetUserId();
        if (string.IsNullOrEmpty(studentId))
            return Unauthorized(new { message = "User identity not found." });

        try
        {
            var result = await _submissionService.CreateSubmissionAsync(studentId, dto);
            if (result == null) 
                return NotFound(new { message = "Assignment not found or invalid ID." });
                
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the submission.", detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubmission(string id, [FromBody] UpdateSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Equals("undefined", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Invalid submission ID provided." });
        }

        var studentId = GetUserId();
        if (string.IsNullOrEmpty(studentId))
            return Unauthorized(new { message = "User identity not found." });

        try
        {
            var result = await _submissionService.UpdateSubmissionAsync(studentId, id, dto);
            if (result == null) 
                return NotFound(new { message = "Submission not found." });
                
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the submission.", detail = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMySubmissions()
    {
        var studentId = GetUserId();
        if (string.IsNullOrEmpty(studentId))
            return Unauthorized(new { message = "User identity not found." });

        try
        {
            var submissions = await _submissionService.GetMySubmissionsAsync(studentId);
            return Ok(submissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving submissions.", detail = ex.Message });
        }
    }

    [HttpGet("teacher")]
    public async Task<IActionResult> GetSubmissionsForTeacher()
    {
        var teacherId = GetUserId();
        if (string.IsNullOrEmpty(teacherId))
            return Unauthorized(new { message = "User identity not found." });

        try
        {
            var submissions = await _submissionService.GetSubmissionsForTeacherAsync(teacherId);
            return Ok(submissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving teacher submissions.", detail = ex.Message });
        }
    }

    [HttpGet("assignment/{assignmentId}")]
    public async Task<IActionResult> GetSubmissionsForAssignment(string assignmentId)
    {
        if (string.IsNullOrWhiteSpace(assignmentId) || assignmentId.Equals("undefined", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Assignment ID is required." });
        }

        try
        {
            var userId = GetUserId();
            var role = User.FindFirstValue(ClaimTypes.Role)?.ToLowerInvariant();

            if (role == "student")
            {
                var studentSubmission = await _submissionService.GetSubmissionByAssignmentAsync(userId, assignmentId);
                return Ok(studentSubmission);
            }

            var submissions = await _submissionService.GetSubmissionsForAssignmentAsync(assignmentId);
            return Ok(submissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving submissions.", detail = ex.Message });
        }
    }

    [HttpPut("{id}/grade")]
    public async Task<IActionResult> GradeSubmission(string id, [FromBody] GradeSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Equals("undefined", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Invalid submission ID provided." });
        }

        try
        {
            var result = await _submissionService.GradeSubmissionAsync(id, dto);
            if (result == null) 
                return NotFound(new { message = "Submission not found." });

            if (dto.Status != null && dto.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                return Ok(new { message = "Submission reset to Pending (deleted). The student can now resubmit.", data = result });
            }
                
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while grading the submission.", detail = ex.Message });
        }
    }
}