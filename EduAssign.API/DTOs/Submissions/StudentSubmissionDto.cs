namespace EduAssign.API.DTOs.Submissions;

public class StudentSubmissionDto
{
    public string Id { get; set; } = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? Marks { get; set; }
    public string? Feedback { get; set; }

    // Joined Subject ID for the table view
    public string SubjectId { get; set; } = string.Empty;

    // Populated Assignment details for the popup modal
    public AssignmentInfoDto? Assignment { get; set; }
}

public class AssignmentInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public int MaximumMarks { get; set; }
    public DateTime Deadline { get; set; }
}