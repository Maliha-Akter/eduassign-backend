using EduAssign.API.DTOs.Assignments;
using EduAssign.API.Models;
using MongoDB.Driver;

namespace EduAssign.API.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IMongoCollection<Assignment> _assignments;

    public AssignmentService(IMongoDatabase database)
    {
        _assignments = database.GetCollection<Assignment>("assignments");
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

    public async Task<Assignment?> GetAssignmentByIdAsync(string id, string teacherId)
    {
        var filter = Builders<Assignment>.Filter.And(
            Builders<Assignment>.Filter.Eq(a => a.Id, id),
            Builders<Assignment>.Filter.Eq(a => a.TeacherId, teacherId)
        );
        return await _assignments.Find(filter).FirstOrDefaultAsync();
    }

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

    // Fetches all published (non-draft) assignments for students
    public async Task<List<Assignment>> GetPublishedAssignmentsAsync()
    {
        var filter = Builders<Assignment>.Filter.Ne(a => a.Status, "Draft");
        return await _assignments.Find(filter).SortByDescending(a => a.CreatedAt).ToListAsync();
    }

    // Fetches a single published assignment for students
    public async Task<Assignment?> GetAssignmentByIdForStudentAsync(string id)
    {
        var filter = Builders<Assignment>.Filter.And(
            Builders<Assignment>.Filter.Eq(a => a.Id, id),
            Builders<Assignment>.Filter.Ne(a => a.Status, "Draft")
        );
        return await _assignments.Find(filter).FirstOrDefaultAsync();
    }
}