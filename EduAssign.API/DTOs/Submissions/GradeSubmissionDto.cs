using System.ComponentModel.DataAnnotations;

namespace EduAssign.API.DTOs.Submissions;

public class GradeSubmissionDto
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Marks must be a non-negative integer.")]
    public int Marks { get; set; }
    
    public string? Feedback { get; set; }
    
    public string Status { get; set; } = "Graded";
}