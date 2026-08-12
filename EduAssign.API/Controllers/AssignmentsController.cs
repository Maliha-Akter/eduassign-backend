using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EduAssign.API.DTOs.Assignments;
using EduAssign.API.Services;
using System.Security.Claims;

namespace EduAssign.API.Controllers;

[ApiController]
[Route("api/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpPost]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(teacherId))
            return Unauthorized(new { message = "Teacher identity could not be determined." });

        var assignment = await _assignmentService.CreateAssignmentAsync(request, teacherId);

        return Ok(new
        {
            message = request.Status == "Draft" ? "Assignment saved as draft." : "Assignment published successfully.",
            assignment
        });
    }

    [HttpGet("my")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> GetMyAssignments()
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(teacherId))
            return Unauthorized(new { message = "Teacher identity could not be determined." });

        var assignments = await _assignmentService.GetAssignmentsByTeacherAsync(teacherId);
        return Ok(assignments);
    }

    // FIXED: Now allows both teachers and students to fetch the assignment details
    [HttpGet("{id}")]
    [Authorize(Roles = "teacher,student")]
    public async Task<IActionResult> GetAssignmentById(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // If the user is a teacher, securely fetch using their teacherId
        if (User.IsInRole("teacher"))
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id, userId);
            if (assignment == null)
                return NotFound(new { message = "Assignment not found or you do not have permission to view it." });

            return Ok(assignment);
        }
        else // Otherwise, the user is a student
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignment == null)
                return NotFound(new { message = "Assignment not found." });

            return Ok(assignment);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> UpdateAssignment(string id, [FromBody] UpdateAssignmentRequest request)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(teacherId))
            return Unauthorized();

        var updatedAssignment = await _assignmentService.UpdateAssignmentAsync(id, request, teacherId);
        
        if (updatedAssignment == null)
            return NotFound(new { message = "Assignment not found or you do not have permission to edit it." });

        return Ok(new
        {
            message = "Assignment updated successfully.",
            assignment = updatedAssignment
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> DeleteAssignment(string id)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(teacherId))
            return Unauthorized();

        var deleted = await _assignmentService.DeleteAssignmentAsync(id, teacherId);
        
        if (!deleted)
            return NotFound(new { message = "Assignment not found or you do not have permission to delete it." });

        return Ok(new { message = "Assignment deleted successfully." });
    }

    [HttpGet("student")]
    [Authorize(Roles = "student")] // SECURED: Added Authorize for students
    public async Task<IActionResult> GetStudentAssignments()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) 
            return Unauthorized();

        var assignments = await _assignmentService.GetAssignmentsForStudentAsync(studentId);
        return Ok(assignments);
    }
}