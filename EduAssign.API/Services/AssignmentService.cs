using EduAssign.API.DTOs.Assignments;
using EduAssign.API.Models;
using MongoDB.Driver;

namespace EduAssign.API.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IMongoCollection<Assignment> _assignments;
    private readonly IMongoCollection<AppUser> _users;

    public AssignmentService(IMongoDatabase database)
    {
        _assignments = database.GetCollection<Assignment>("assignments");
        _users = database.GetCollection<AppUser>("user");
    }

    public async Task<Assignment> CreateAssignmentAsync(CreateAssignmentRequest request, string teacherId)
    {
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

    public async Task<List<Assignment>> GetAssignmentsByTeacherAsync(string teacherId)
    {
        var filter = Builders<Assignment>.Filter.Eq(a => a.TeacherId, teacherId);
        return await _assignments.Find(filter).SortByDescending(a => a.CreatedAt).ToListAsync();
    }

    // SAFE UPDATE: Only updates specific fields, preserves CreatedAt, TeacherId, etc.
    public async Task<Assignment?> UpdateAssignmentAsync(string id, UpdateAssignmentRequest request, string teacherId)
    {
        var filter = Builders<Assignment>.Filter.And(
            Builders<Assignment>.Filter.Eq(a => a.Id, id),
            Builders<Assignment>.Filter.Eq(a => a.TeacherId, teacherId)
        );

        var update = Builders<Assignment>.Update
            .Set(a => a.Title, request.Title)
            .Set(a => a.Description, request.Description)
            .Set(a => a.ClassId, request.ClassId)
            .Set(a => a.SubjectId, request.SubjectId)
            .Set(a => a.Deadline, request.Deadline)
            .Set(a => a.MaximumMarks, request.MaximumMarks)
            .Set(a => a.Status, request.Status)
            .Set(a => a.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<Assignment>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await _assignments.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task<bool> DeleteAssignmentAsync(string id, string teacherId)
    {
        var filter = Builders<Assignment>.Filter.And(
            Builders<Assignment>.Filter.Eq(a => a.Id, id),
            Builders<Assignment>.Filter.Eq(a => a.TeacherId, teacherId)
        );

        var result = await _assignments.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<List<Assignment>> GetAssignmentsForStudentAsync(string studentId)
    {
        var student = await _users.Find(u => u.Id == studentId).FirstOrDefaultAsync();
        
        if (student == null || string.IsNullOrEmpty(student.Class))
            return new List<Assignment>();

        var classId = $"class_{student.Class}";

        var filter = Builders<Assignment>.Filter.Eq(a => a.ClassId, classId);

        return await _assignments.Find(filter).SortByDescending(a => a.CreatedAt).ToListAsync();
    }

    // 👈 NEW/UPDATED: For general lookup (used by students) fetches Teacher Name and Email
    public async Task<Assignment?> GetAssignmentByIdAsync(string id)
    {
        var assignment = await _assignments.Find(a => a.Id == id).FirstOrDefaultAsync();
        
        if (assignment != null)
        {
            var teacher = await _users.Find(u => u.Id == assignment.TeacherId).FirstOrDefaultAsync();
            if (teacher != null)
            {
                assignment.TeacherName = teacher.Name;
                assignment.TeacherEmail = teacher.Email; 
            }
        }
        
        return assignment;
    }

    // 👈 NEW/UPDATED: For teacher-restricted lookup fetches Teacher Name and Email
    public async Task<Assignment?> GetAssignmentByIdAsync(string id, string teacherId)
    {
        var filter = Builders<Assignment>.Filter.And(
            Builders<Assignment>.Filter.Eq(a => a.Id, id),
            Builders<Assignment>.Filter.Eq(a => a.TeacherId, teacherId)
        );
        var assignment = await _assignments.Find(filter).FirstOrDefaultAsync();
        
        if (assignment != null)
        {
            var teacher = await _users.Find(u => u.Id == assignment.TeacherId).FirstOrDefaultAsync();
            if (teacher != null)
            {
                assignment.TeacherName = teacher.Name;
                assignment.TeacherEmail = teacher.Email; 
            }
        }

        return assignment;
    }
}