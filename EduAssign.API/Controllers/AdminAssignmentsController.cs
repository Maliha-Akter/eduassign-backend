using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EduAssign.API.Data;
using EduAssign.API.Models;

namespace EduAssign.API.Controllers
{
    [Route("api/admin/assignments")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminAssignmentsController : ControllerBase
    {
        private readonly IMongoCollection<Assignment> _assignments;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Submission> _submissions;

        public AdminAssignmentsController(MongoDbContext context)
        {
            _assignments = context.GetCollection<Assignment>("assignments");
            _users = context.GetCollection<User>("user");
            _submissions = context.GetCollection<Submission>("submissions");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssignments()
        {
            var assignments = await _assignments.Find(_ => true).ToListAsync();
            var users = await _users.Find(_ => true).ToListAsync();
            var submissions = await _submissions.Find(_ => true).ToListAsync();

            var result = assignments.Select(a =>
            {
                // Use .ToString() safety to handle string vs ObjectId mismatches
                var assignmentSubs = submissions
                    .Where(s => s.AssignmentId != null && s.AssignmentId.ToString() == a.Id?.ToString())
                    .ToList();

                var teacher = users.FirstOrDefault(u => u.Id != null && u.Id.ToString() == a.TeacherId?.ToString());

                return new
                {
                    Id = a.Id?.ToString(),
                    a.Title,
                    a.Description,
                    Deadline = a.Deadline,
                    a.CreatedAt,
                    TeacherName = teacher?.Name ?? "Unknown Teacher",
                    ClassName = a.ClassId,
                    SubjectName = a.SubjectId,
                    SubmissionsCount = assignmentSubs.Count,
                    Submissions = assignmentSubs.Select(s =>
                    {
                        var student = users.FirstOrDefault(u => u.Id != null && u.Id.ToString() == s.StudentId?.ToString());
                        return new
                        {
                            Id = s.Id?.ToString(),
                            AssignmentId = s.AssignmentId?.ToString(),
                            StudentName = student?.Name ?? "Unknown Student",
                            s.Answer,
                            s.Marks,
                            s.Feedback,
                            s.Status,
                            s.SubmittedAt,
                            s.UpdatedAt
                        };
                    }).ToList()
                };
            }).OrderByDescending(a => a.CreatedAt);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(string id)
        {
            var result = await _assignments.DeleteOneAsync(a => a.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            
            // Clean up related submissions
            await _submissions.DeleteManyAsync(s => s.AssignmentId == id);

            return Ok(new { message = "Assignment and its submissions deleted by Admin" });
        }
    }
}