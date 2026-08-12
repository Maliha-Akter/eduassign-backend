namespace EduAssign.API.DTOs.Submissions;

public class TeacherSubmissionDto
{
    public string Id { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;
    public string AssignmentTitle { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public DateTime Deadline { get; set; }
    public string Answer { get; set; } = string.Empty;
    public int? Marks { get; set; }
    public int MaxMarks { get; set; }
    public string? Feedback { get; set; }
    public string Status { get; set; } = "Submitted";
}