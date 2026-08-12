namespace EduAssign.API.DTOs.Assignments;

public class UpdateAssignmentRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaximumMarks { get; set; }
    public string Status { get; set; } = "Draft";
}