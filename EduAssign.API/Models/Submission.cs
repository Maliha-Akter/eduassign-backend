using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models
{
    public class Submission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string AssignmentId { get; set; } = null!;

        public string StudentId { get; set; } = null!; // BetterAuth User ID

        public string Answer { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Submitted"; // Submitted, Late, Graded

        public int? Marks { get; set; }

        public string? Feedback { get; set; }
    }
}