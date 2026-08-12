using System.ComponentModel.DataAnnotations;

namespace EduAssign.API.DTOs.Submissions
{
    public class GradeSubmissionDto
    {
        [Required]
        public int Marks { get; set; }
        public string? Feedback { get; set; }
    }
}