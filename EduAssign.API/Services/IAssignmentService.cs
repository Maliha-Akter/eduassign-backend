using EduAssign.API.DTOs.Assignments;
using EduAssign.API.Models;

namespace EduAssign.API.Services;

public interface IAssignmentService
{
    Task<Assignment> CreateAssignmentAsync(CreateAssignmentRequest request, string teacherId);
    Task<List<Assignment>> GetAssignmentsByTeacherAsync(string teacherId);
    Task<Assignment?> GetAssignmentByIdAsync(string id, string teacherId);
    Task<Assignment?> UpdateAssignmentAsync(string id, UpdateAssignmentRequest request, string teacherId);
    Task<bool> DeleteAssignmentAsync(string id, string teacherId);
}