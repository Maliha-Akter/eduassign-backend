using System.ComponentModel.DataAnnotations;

namespace EduAssign.API.DTOs.Submissions;

public class UpdateSubmissionDto
{
    [Required]
    public string Answer { get; set; } = string.Empty;
}