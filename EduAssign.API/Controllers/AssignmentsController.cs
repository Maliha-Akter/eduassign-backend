using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EduAssign.API.DTOs.Assignments;
using EduAssign.API.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EduAssign.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")] // Rules 1 & 6: Only authenticated teachers
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
        {
            // The validator handles model state automatically if registered, or you can manually check ModelState

            // Extract the Teacher's ID from the JWT claims
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized();

            var createdAssignment = await _assignmentService.CreateAssignmentAsync(request, teacherId);

            return CreatedAtAction(nameof(GetAssignmentById), new { id = createdAssignment.Id }, new {
                message = request.Status == "Draft" ? "Assignment saved as draft." : "Assignment published successfully.",
                assignment = createdAssignment
            });
        }

        // Placeholder for GET route
        [HttpGet("{id}")]
        public IActionResult GetAssignmentById(string id)
        {
            return Ok();
        }
    }
}