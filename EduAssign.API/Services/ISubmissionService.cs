using EduAssign.API.Models;
using EduAssign.API.DTOs.Submissions;

namespace EduAssign.API.Services
{
    public interface ISubmissionService
    {
        Task<Submission?> CreateSubmissionAsync(string studentId, CreateSubmissionDto dto);
        Task<Submission?> UpdateSubmissionAsync(string studentId, string submissionId, UpdateSubmissionDto dto);
        Task<List<Submission>> GetMySubmissionsAsync(string studentId);
        Task<Submission?> GetSubmissionByAssignmentAsync(string studentId, string assignmentId);
        
        // For Teachers
        Task<List<Submission>> GetSubmissionsForAssignmentAsync(string assignmentId);
        Task<Submission?> GradeSubmissionAsync(string submissionId, GradeSubmissionDto dto);
    }
}