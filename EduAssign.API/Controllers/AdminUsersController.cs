using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EduAssign.API.Data;
using EduAssign.API.Models;

namespace EduAssign.API.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public AdminUsersController(MongoDbContext context)
        {
            _users = context.GetCollection<User>("user");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _users.Find(_ => true).ToListAsync();
            
            var result = users.Select(u => new 
            { 
                u.Id, 
                u.Name, 
                u.Email, 
                u.Role,
                u.IsBlocked
            });
            
            return Ok(result);
        }

        [HttpPatch("{id}/role")]
        public async Task<IActionResult> ChangeRole(string id, [FromBody] ChangeRoleDto dto)
        {
            var update = Builders<User>.Update.Set(u => u.Role, dto.Role);
            var result = await _users.UpdateOneAsync(u => u.Id == id, update);

            // Changed from ModifiedCount to MatchedCount
            if (result.MatchedCount == 0) return NotFound(new { message = "User not found" });
            return Ok(new { message = "Role updated successfully" });
        }

        [HttpPatch("{id}/block")]
        public async Task<IActionResult> ToggleBlock(string id, [FromBody] ToggleBlockDto dto)
        {
            var update = Builders<User>.Update.Set(u => u.IsBlocked, dto.IsBlocked);
            var result = await _users.UpdateOneAsync(u => u.Id == id, update);

            // Changed from ModifiedCount to MatchedCount
            if (result.MatchedCount == 0) return NotFound(new { message = "User not found" });
            return Ok(new { message = dto.IsBlocked ? "User blocked successfully" : "User unblocked successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            if (result.DeletedCount == 0) return NotFound(new { message = "User not found" });
            return Ok(new { message = "User deleted" });
        }
    }

    public class ChangeRoleDto
    {
        public string Role { get; set; } = string.Empty;
    }

    public class ToggleBlockDto
    {
        public bool IsBlocked { get; set; }
    }
}