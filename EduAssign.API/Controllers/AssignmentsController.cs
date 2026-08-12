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

    [HttpGet("{id}")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> GetAssignmentById(string id)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(teacherId))
            return Unauthorized();

        var assignment = await _assignmentService.GetAssignmentByIdAsync(id, teacherId);
        if (assignment == null)
            return NotFound(new { message = "Assignment not found or you do not have permission to view it." });

        return Ok(assignment);
    }

    // NEW: Update Endpoint
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

    // NEW: Delete Endpoint
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
}