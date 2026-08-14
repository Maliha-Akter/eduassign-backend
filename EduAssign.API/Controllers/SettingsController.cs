using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EduAssign.API.Data;
using EduAssign.API.Models;

namespace EduAssign.API.Controllers
{
    [Route("api/admin/settings")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SettingsController : ControllerBase
    {
        private readonly IMongoCollection<AppSettings> _settings;

        public SettingsController(MongoDbContext context)
        {
            _settings = context.GetCollection<AppSettings>("Settings");
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _settings.Find(_ => true).FirstOrDefaultAsync();
            if (settings == null)
            {
                // Create default settings if none exist
                settings = new AppSettings();
                await _settings.InsertOneAsync(settings);
            }
            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] AppSettings updatedSettings)
        {
            var existingSettings = await _settings.Find(_ => true).FirstOrDefaultAsync();
            
            if (existingSettings == null)
            {
                await _settings.InsertOneAsync(updatedSettings);
            }
            else
            {
                updatedSettings.Id = existingSettings.Id; // Keep the same MongoDB Document ID
                await _settings.ReplaceOneAsync(s => s.Id == existingSettings.Id, updatedSettings);
            }

            return Ok(updatedSettings);
        }
    }
}