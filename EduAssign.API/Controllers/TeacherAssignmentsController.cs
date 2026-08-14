using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EduAssign.API.Data;
using EduAssign.API.Models;

namespace EduAssign.API.Controllers
{
    [Route("api/admin/teacher-assignments")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class TeacherAssignmentsController : ControllerBase
    {
        private readonly IMongoCollection<TeacherAssignment> _assignments;
        private readonly IMongoCollection<User> _users;

        public TeacherAssignmentsController(MongoDbContext context)
        {
            _assignments = context.GetCollection<TeacherAssignment>("TeacherAssignments");
            _users = context.GetCollection<User>("user");
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _assignments.Find(_ => true).ToListAsync();
            var teacherIds = assignments.Select(a => a.TeacherId).Distinct().ToList();

            // Fetch the user documents for all assigned teachers
            var teachers = await _users.Find(u => teacherIds.Contains(u.Id)).ToListAsync();

            // Map TeacherId -> User object
            var teacherMap = teachers.ToDictionary(u => u.Id, u => u);

            var result = assignments.Select(a =>
            {
                var hasTeacher = teacherMap.TryGetValue(a.TeacherId, out var teacher);

                return new
                {
                    a.Id,
                    a.TeacherId,
                    TeacherName = hasTeacher ? teacher?.Name : "Unknown Teacher",
                    // Checks PrimarySubject first, then falls back to Subject, then "N/A"
                    PrimarySubject = hasTeacher ? (teacher?.PrimarySubject ?? teacher?.Subject ?? "N/A") : "N/A",
                    a.AssignedClass,
                    a.Section,
                    a.CreatedAt
                };
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] TeacherAssignment newAssignment)
        {
            newAssignment.CreatedAt = DateTime.UtcNow;
            // The PrimarySubject is now automatically populated by the frontend!
            await _assignments.InsertOneAsync(newAssignment);
            return Ok(newAssignment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(string id, [FromBody] TeacherAssignment updatedData)
        {
            var update = Builders<TeacherAssignment>.Update
                .Set(a => a.TeacherId, updatedData.TeacherId)
                .Set(a => a.AssignedClass, updatedData.AssignedClass)
                .Set(a => a.Section, updatedData.Section)
                .Set(a => a.PrimarySubject, updatedData.PrimarySubject); // Pushing the subject update

            var result = await _assignments.UpdateOneAsync(a => a.Id == id, update);

            if (result.MatchedCount == 0) return NotFound(new { message = "Assignment not found" });

            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(string id)
        {
            var result = await _assignments.DeleteOneAsync(a => a.Id == id);
            if (result.DeletedCount == 0) return NotFound(new { message = "Assignment not found" });
            return Ok(new { message = "Assignment deleted" });
        }
    }
}