using EduAssign.API.Models;
using EduAssign.API.DTOs.Submissions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EduAssign.API.Services;

public class SubmissionService : ISubmissionService
{
    private readonly IMongoCollection<Submission> _submissions;
    private readonly IMongoCollection<Assignment> _assignments;
    private readonly IMongoCollection<AppUser> _users;

    public SubmissionService(IMongoDatabase database)
    {
        _submissions = database.GetCollection<Submission>("submissions");
        _assignments = database.GetCollection<Assignment>("assignments");
        _users = database.GetCollection<AppUser>("user");
    }

    public async Task<Submission?> CreateSubmissionAsync(string studentId, CreateSubmissionDto dto)
    {
        if (!ObjectId.TryParse(dto.AssignmentId, out _) || !ObjectId.TryParse(studentId, out _))
            return null;

        var assignmentFilter = Builders<Assignment>.Filter.Eq(a => a.Id, dto.AssignmentId);
        var assignment = await _assignments.Find(assignmentFilter).FirstOrDefaultAsync();
        if (assignment == null) return null;

        var existingFilter = Builders<Submission>.Filter.And(
            Builders<Submission>.Filter.Eq(s => s.StudentId, studentId),
            Builders<Submission>.Filter.Eq(s => s.AssignmentId, dto.AssignmentId)
        );
        var existing = await _submissions.Find(existingFilter).FirstOrDefaultAsync();
        if (existing != null) 
            throw new InvalidOperationException("Assignment has already been submitted.");

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = studentId,
            Answer = dto.Answer,
            Status = DateTime.UtcNow > assignment.Deadline ? "Late" : "Submitted",
            SubmittedAt = DateTime.UtcNow
        };

        await _submissions.InsertOneAsync(submission);
        return submission;
    }

    public async Task<Submission?> UpdateSubmissionAsync(string studentId, string submissionId, UpdateSubmissionDto dto)
    {
        if (!ObjectId.TryParse(submissionId, out _) || !ObjectId.TryParse(studentId, out _))
            return null;

        var submissionFilter = Builders<Submission>.Filter.And(
            Builders<Submission>.Filter.Eq(s => s.Id, submissionId),
            Builders<Submission>.Filter.Eq(s => s.StudentId, studentId)
        );
        var submission = await _submissions.Find(submissionFilter).FirstOrDefaultAsync();
        if (submission == null) return null;

        var assignmentFilter = Builders<Assignment>.Filter.Eq(a => a.Id, submission.AssignmentId);
        var assignment = await _assignments.Find(assignmentFilter).FirstOrDefaultAsync();
        
        if (assignment != null && DateTime.UtcNow > assignment.Deadline)
            throw new InvalidOperationException("Deadline passed. Cannot update submission.");

        var update = Builders<Submission>.Update
            .Set(s => s.Answer, dto.Answer)
            .Set(s => s.UpdatedAt, DateTime.UtcNow);

        await _submissions.UpdateOneAsync(submissionFilter, update);
        
        submission.Answer = dto.Answer;
        return submission;
    }

    public async Task<List<StudentSubmissionDto>> GetMySubmissionsAsync(string studentId)
    {
        if (!ObjectId.TryParse(studentId, out _))
            return new List<StudentSubmissionDto>();

        // 1. Fetch student's submissions sorted by date
        var filter = Builders<Submission>.Filter.Eq(s => s.StudentId, studentId);
        var submissions = await _submissions.Find(filter)
            .SortByDescending(s => s.SubmittedAt)
            .ToListAsync();

        if (!submissions.Any()) 
            return new List<StudentSubmissionDto>();

        // 2. Fetch all matching assignments in a single batch query
        var assignmentIds = submissions.Select(s => s.AssignmentId).Distinct().ToList();
        var assignmentFilter = Builders<Assignment>.Filter.In(a => a.Id, assignmentIds);
        var assignments = await _assignments.Find(assignmentFilter).ToListAsync();
        var assignmentDict = assignments.ToDictionary(a => a.Id);

        // 3. Map submissions and joined assignment details into DTOs
        var result = new List<StudentSubmissionDto>(submissions.Count);

        foreach (var sub in submissions)
        {
            assignmentDict.TryGetValue(sub.AssignmentId, out var assignment);

            result.Add(new StudentSubmissionDto
            {
                Id = sub.Id,
                AssignmentId = sub.AssignmentId,
                StudentId = sub.StudentId,
                Answer = sub.Answer ?? string.Empty,
                SubmittedAt = sub.SubmittedAt,
                UpdatedAt = sub.UpdatedAt,
                Status = string.IsNullOrEmpty(sub.Status) 
                    ? (sub.Marks.HasValue ? "Graded" : "Submitted") 
                    : sub.Status,
                Marks = sub.Marks,
                Feedback = sub.Feedback,
                SubjectId = assignment?.SubjectId ?? "N/A",
                Assignment = assignment != null ? new AssignmentInfoDto
                {
                    Id = assignment.Id,
                    Title = assignment.Title ?? string.Empty,
                    Description = assignment.Description ?? string.Empty,
                    SubjectId = assignment.SubjectId ?? string.Empty,
                    ClassId = assignment.ClassId ?? string.Empty,
                    MaximumMarks = assignment.MaximumMarks,
                    Deadline = assignment.Deadline
                } : null
            });
        }

        return result;
    }

    public async Task<Submission?> GetSubmissionByAssignmentAsync(string studentId, string assignmentId)
    {
        if (!ObjectId.TryParse(studentId, out _) || !ObjectId.TryParse(assignmentId, out _))
            return null;

        var filter = Builders<Submission>.Filter.And(
            Builders<Submission>.Filter.Eq(s => s.StudentId, studentId),
            Builders<Submission>.Filter.Eq(s => s.AssignmentId, assignmentId)
        );
        return await _submissions.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Submission>> GetSubmissionsForAssignmentAsync(string assignmentId)
    {
        if (!ObjectId.TryParse(assignmentId, out _))
            return new List<Submission>();

        var filter = Builders<Submission>.Filter.Eq(s => s.AssignmentId, assignmentId);
        return await _submissions.Find(filter).ToListAsync();
    }

    public async Task<List<TeacherSubmissionDto>> GetSubmissionsForTeacherAsync(string teacherId)
    {
        if (!ObjectId.TryParse(teacherId, out _))
            return new List<TeacherSubmissionDto>();

        var assignmentFilter = Builders<Assignment>.Filter.Eq(a => a.TeacherId, teacherId);
        var teacherAssignments = await _assignments.Find(assignmentFilter).ToListAsync();

        if (!teacherAssignments.Any()) 
            return new List<TeacherSubmissionDto>();

        var assignmentMap = teacherAssignments.ToDictionary(a => a.Id);
        var assignmentIds = teacherAssignments.Select(a => a.Id).ToList();

        var submissionFilter = Builders<Submission>.Filter.In(s => s.AssignmentId, assignmentIds);
        var submissions = await _submissions
            .Find(submissionFilter)
            .SortByDescending(s => s.SubmittedAt)
            .ToListAsync();

        var studentIds = submissions.Select(s => s.StudentId).Distinct().ToList();
        var userFilter = Builders<AppUser>.Filter.In(u => u.Id, studentIds);
        var students = await _users.Find(userFilter).ToListAsync();
        
        var studentMap = students.ToDictionary(u => u.Id);
        var result = new List<TeacherSubmissionDto>(submissions.Count);

        foreach (var sub in submissions)
        {
            assignmentMap.TryGetValue(sub.AssignmentId, out var assignment);
            studentMap.TryGetValue(sub.StudentId, out var student);

            result.Add(new TeacherSubmissionDto
            {
                Id = sub.Id,
                StudentId = sub.StudentId,
                StudentName = student?.Name ?? "Unknown Student",
                StudentEmail = student?.Email ?? "N/A",
                AssignmentId = sub.AssignmentId,
                AssignmentTitle = assignment?.Title ?? "Untitled Assignment",
                ClassName = assignment?.ClassId?.Replace("class_", "Class ") ?? "N/A",
                SubmittedAt = sub.SubmittedAt,
                Deadline = assignment?.Deadline ?? DateTime.MinValue,
                Answer = sub.Answer ?? string.Empty,
                Marks = sub.Marks,
                MaxMarks = assignment?.MaximumMarks ?? 100,
                Feedback = sub.Feedback,
                Status = string.IsNullOrEmpty(sub.Status) 
                    ? (sub.Marks.HasValue ? "Graded" : "Submitted") 
                    : sub.Status
            });
        }

        return result;
    }

    public async Task<Submission?> GradeSubmissionAsync(string submissionId, GradeSubmissionDto dto)
    {
        if (!ObjectId.TryParse(submissionId, out _))
            return null;

        var filter = Builders<Submission>.Filter.Eq(s => s.Id, submissionId);
        var existing = await _submissions.Find(filter).FirstOrDefaultAsync();
        if (existing == null) return null;

        if (dto.Status != null && dto.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            await _submissions.DeleteOneAsync(filter);
            return existing;
        }

        var statusToSet = string.IsNullOrEmpty(dto.Status) ? "Graded" : dto.Status;

        var update = Builders<Submission>.Update
            .Set(s => s.Marks, dto.Marks)
            .Set(s => s.Feedback, dto.Feedback)
            .Set(s => s.Status, statusToSet)
            .Set(s => s.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<Submission> { ReturnDocument = ReturnDocument.After };
        return await _submissions.FindOneAndUpdateAsync(filter, update, options);
    }
}