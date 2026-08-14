using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EduAssign.API.Data;
using EduAssign.API.Models;

namespace EduAssign.API.Controllers
{
    [Route("api/admin/courses")]
    [ApiController]
    // [Authorize(Roles = "admin")]
    [AllowAnonymous]
    public class CoursesController : ControllerBase
    {
        private readonly IMongoCollection<Course> _courses;

        public CoursesController(MongoDbContext context)
        {
            _courses = context.GetCollection<Course>("Courses");
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courses.Find(_ => true).ToListAsync();
            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            await _courses.InsertOneAsync(course);
            return Ok(course);
        }

        // --- NEW UPDATE ENDPOINT ---
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(string id, [FromBody] UpdateCourseDto dto)
        {
            var update = Builders<Course>.Update
                .Set(c => c.Name, dto.Name)
                .Set(c => c.Code, dto.Code);

            var result = await _courses.UpdateOneAsync(c => c.Id == id, update);

            if (result.MatchedCount == 0) return NotFound(new { message = "Course not found" });
            return Ok(new { message = "Course updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            var result = await _courses.DeleteOneAsync(c => c.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return Ok(new { message = "Course deleted" });
        }
    }

    public class UpdateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}