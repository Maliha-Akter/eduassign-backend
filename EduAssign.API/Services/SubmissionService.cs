using EduAssign.API.Models;
using EduAssign.API.DTOs.Submissions;
using MongoDB.Driver;

namespace EduAssign.API.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IMongoCollection<Submission> _submissions;
        private readonly IMongoCollection<Assignment> _assignments;

        public SubmissionService(IMongoDatabase database)
        {
            _submissions = database.GetCollection<Submission>("submissions");
            _assignments = database.GetCollection<Assignment>("assignments");
        }

        public async Task<Submission?> CreateSubmissionAsync(string studentId, CreateSubmissionDto dto)
        {
            var assignment = await _assignments.Find(a => a.Id == dto.AssignmentId).FirstOrDefaultAsync();
            if (assignment == null) return null;

            // Check if already submitted
            var existing = await _submissions.Find(s => s.StudentId == studentId && s.AssignmentId == dto.AssignmentId).FirstOrDefaultAsync();
            if (existing != null) throw new Exception("Already submitted");

            var submission = new Submission
            {
                AssignmentId = dto.AssignmentId,
                StudentId = studentId,
                Answer = dto.Answer,
                Status = DateTime.UtcNow > assignment.Deadline ? "Late" : "Submitted"
            };

            await _submissions.InsertOneAsync(submission);
            return submission;
        }

        public async Task<Submission?> UpdateSubmissionAsync(string studentId, string submissionId, UpdateSubmissionDto dto)
        {
            var submission = await _submissions.Find(s => s.Id == submissionId && s.StudentId == studentId).FirstOrDefaultAsync();
            if (submission == null) return null;

            var assignment = await _assignments.Find(a => a.Id == submission.AssignmentId).FirstOrDefaultAsync();
            if (assignment != null && DateTime.UtcNow > assignment.Deadline)
                throw new Exception("Deadline passed. Cannot update.");

            var update = Builders<Submission>.Update
                .Set(s => s.Answer, dto.Answer)
                .Set(s => s.UpdatedAt, DateTime.UtcNow);

            await _submissions.UpdateOneAsync(s => s.Id == submissionId, update);
            submission.Answer = dto.Answer;
            return submission;
        }

        public async Task<List<Submission>> GetMySubmissionsAsync(string studentId)
        {
            return await _submissions.Find(s => s.StudentId == studentId)
                .SortByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<Submission?> GetSubmissionByAssignmentAsync(string studentId, string assignmentId)
        {
            return await _submissions.Find(s => s.StudentId == studentId && s.AssignmentId == assignmentId).FirstOrDefaultAsync();
        }

        public async Task<List<Submission>> GetSubmissionsForAssignmentAsync(string assignmentId)
        {
            return await _submissions.Find(s => s.AssignmentId == assignmentId).ToListAsync();
        }

        public async Task<Submission?> GradeSubmissionAsync(string submissionId, GradeSubmissionDto dto)
        {
            var update = Builders<Submission>.Update
                .Set(s => s.Marks, dto.Marks)
                .Set(s => s.Feedback, dto.Feedback)
                .Set(s => s.Status, "Graded")
                .Set(s => s.UpdatedAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<Submission> { ReturnDocument = ReturnDocument.After };
            return await _submissions.FindOneAndUpdateAsync(s => s.Id == submissionId, update, options);
        }
    }
}