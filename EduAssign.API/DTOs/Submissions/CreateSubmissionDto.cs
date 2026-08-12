using System.ComponentModel.DataAnnotations;

namespace EduAssign.API.DTOs.Submissions
{
    public class CreateSubmissionDto
    {
        [Required]
        public string AssignmentId { get; set; } = null!;

        [Required]
        public string Answer { get; set; } = null!;
    }
}