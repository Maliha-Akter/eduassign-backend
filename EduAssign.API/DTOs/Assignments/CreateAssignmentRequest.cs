using System;

namespace EduAssign.API.DTOs.Assignments
{
    public class CreateAssignmentRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ClassId { get; set; } = null!;
        public string SubjectId { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public int MaximumMarks { get; set; }
        public string Status { get; set; } = null!;
    }
}