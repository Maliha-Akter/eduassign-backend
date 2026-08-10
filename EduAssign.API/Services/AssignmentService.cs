using EduAssign.API.Models;
using EduAssign.API.DTOs.Assignments;
using MongoDB.Driver;
using System.Threading.Tasks;
using System;

namespace EduAssign.API.Services
{
    public interface IAssignmentService
    {
        Task<Assignment> CreateAssignmentAsync(CreateAssignmentRequest request, string teacherId);
    }

    public class AssignmentService : IAssignmentService
    {
        private readonly IMongoCollection<Assignment> _assignments;
        // Assume you inject collections for Classes and Subjects to verify existence
        // private readonly IMongoCollection<Class> _classes; 
        
        public AssignmentService(IMongoDatabase database)
        {
            _assignments = database.GetCollection<Assignment>("assignments");
        }

        public async Task<Assignment> CreateAssignmentAsync(CreateAssignmentRequest request, string teacherId)
        {
            // TODO: (Rule 4 & 5) Await checks to ensure request.ClassId and request.SubjectId exist in DB
            
            var assignment = new Assignment
            {
                TeacherId = teacherId,
                Title = request.Title,
                Description = request.Description,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                Deadline = request.Deadline,
                MaximumMarks = request.MaximumMarks,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _assignments.InsertOneAsync(assignment);
            return assignment;
        }
    }
}