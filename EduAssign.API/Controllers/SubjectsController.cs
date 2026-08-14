using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EduAssign.API.Data;
using EduAssign.API.Models;

namespace EduAssign.API.Controllers
{
    [Route("api/admin/subjects")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class SubjectsController : ControllerBase
    {
        private readonly IMongoCollection<Subject> _subjects;
        private readonly IMongoCollection<Course> _courses;

        public SubjectsController(MongoDbContext context)
        {
            _subjects = context.GetCollection<Subject>("Subjects");
            _courses = context.GetCollection<Course>("Courses");
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _subjects.Find(_ => true).ToListAsync();
            var courses = await _courses.Find(_ => true).ToListAsync();

            // Map course names manually since MongoDB doesn't have SQL JOINs
            var result = subjects.Select(s => new {
                s.Id, 
                s.Name, 
                s.Code, 
                s.CourseId, 
                CourseName = courses.FirstOrDefault(c => c.Id == s.CourseId)?.Name ?? "Unknown Course"
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] Subject subject)
        {
            await _subjects.InsertOneAsync(subject);
            return Ok(subject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(string id)
        {
            var result = await _subjects.DeleteOneAsync(s => s.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return Ok(new { message = "Subject deleted" });
        }
    }
}