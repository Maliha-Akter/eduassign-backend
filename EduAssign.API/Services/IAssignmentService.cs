using EduAssign.API.DTOs.Assignments;
using EduAssign.API.Models;

namespace EduAssign.API.Services;

public interface IAssignmentService
{
    Task<Assignment> CreateAssignmentAsync(CreateAssignmentRequest request, string teacherId);
    Task<List<Assignment>> GetAssignmentsByTeacherAsync(string teacherId);
    
    // For teachers (needs 2 parameters)
    Task<Assignment?> GetAssignmentByIdAsync(string id, string teacherId);
    
    // ADD THIS LINE FOR STUDENTS (needs 1 parameter)
    Task<Assignment?> GetAssignmentByIdAsync(string id); 
    
    Task<Assignment?> UpdateAssignmentAsync(string id, UpdateAssignmentRequest request, string teacherId);
    Task<bool> DeleteAssignmentAsync(string id, string teacherId);
    Task<List<Assignment>> GetAssignmentsForStudentAsync(string studentId);
}