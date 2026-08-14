using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EduAssign.API.Data;
using EduAssign.API.Models;

namespace EduAssign.API.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    [Authorize(Roles = "teacher")] // Role is lowercase "teacher" based on DB
    public class TeacherPortalController : ControllerBase
    {
        private readonly IMongoCollection<Assignment> _assignments;
        private readonly IMongoCollection<Submission> _submissions;
        private readonly IMongoCollection<User> _users;

        public TeacherPortalController(MongoDbContext context)
        {
            _assignments = context.GetCollection<Assignment>("assignments");
            _submissions = context.GetCollection<Submission>("submissions");
            _users = context.GetCollection<User>("user");
        }

        private string GetTeacherId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        // 1. Get subjects assigned to this teacher
        [HttpGet("my-subjects")]
        public async Task<IActionResult> GetMySubjects()
        {
            var teacherId = GetTeacherId();
            
            var assignments = await _assignments.Find(a => a.TeacherId == teacherId).ToListAsync();
            
            var result = assignments
                .Select(a => new { a.ClassId, a.SubjectId })
                .Distinct()
                .ToList();

            return Ok(result);
        }

        // 2. Get assignments created by this teacher
        [HttpGet("assignments")]
        public async Task<IActionResult> GetMyAssignments()
        {
            var teacherId = GetTeacherId();
            var assignments = await _assignments.Find(a => a.TeacherId == teacherId).ToListAsync();

            var result = assignments.Select(a => new {
                a.Id,
                a.Title,
                a.Deadline,
                a.ClassId,
                a.SubjectId,
                a.Status
            });

            return Ok(result);
        }

        // 3. Create a new assignment
        [HttpPost("assignments")]
        public async Task<IActionResult> CreateAssignment([FromBody] Assignment assignment)
        {
            assignment.TeacherId = GetTeacherId();
            assignment.CreatedAt = DateTime.UtcNow;
            assignment.UpdatedAt = DateTime.UtcNow;
            assignment.Status = "Published";
            
            await _assignments.InsertOneAsync(assignment);
            return Ok(assignment);
        }

        // 4. View submissions for a specific assignment
        [HttpGet("assignments/{assignmentId}/submissions")]
        public async Task<IActionResult> GetSubmissions(string assignmentId)
        {
            var teacherId = GetTeacherId();
            var assignment = await _assignments.Find(a => a.Id == assignmentId && a.TeacherId == teacherId).FirstOrDefaultAsync();
            if (assignment == null) return Unauthorized("You do not own this assignment.");

            var submissions = await _submissions.Find(s => s.AssignmentId == assignmentId).ToListAsync();
            
            var studentIds = submissions.Where(s => !string.IsNullOrEmpty(s.StudentId)).Select(s => s.StudentId).Distinct().ToList();
            var students = await _users.Find(u => studentIds.Contains(u.Id)).ToListAsync();

            var result = submissions.Select(s => new {
                s.Id,
                s.Answer,
                s.SubmittedAt,
                s.Marks,
                s.Feedback,
                s.Status,
                StudentName = students.FirstOrDefault(u => u.Id == s.StudentId)?.Name ?? "Unknown Student"
            });

            return Ok(result);
        }

        // 5. Grade a submission
        [HttpPatch("submissions/{submissionId}/grade")]
        public async Task<IActionResult> GradeSubmission(string submissionId, [FromBody] GradeDto dto)
        {
            var update = Builders<Submission>.Update
                .Set(s => s.Marks, dto.Marks)
                .Set(s => s.Feedback, dto.Feedback)
                .Set(s => s.Status, "Graded")
                .Set(s => s.UpdatedAt, DateTime.UtcNow);

            var result = await _submissions.UpdateOneAsync(s => s.Id == submissionId, update);
            if (result.ModifiedCount == 0) return NotFound("Submission not found.");

            return Ok(new { message = "Marks updated successfully" });
        }
    }

    public class GradeDto
    {
        public int Marks { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}