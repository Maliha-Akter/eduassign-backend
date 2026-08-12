using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models;

public class Submission
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("AssignmentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string AssignmentId { get; set; } = null!;

    [BsonElement("StudentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string StudentId { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public int? Marks { get; set; }

    public string? Feedback { get; set; }

    public string Status { get; set; } = "Submitted";

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [BsonIgnore]
    public string? Subject { get; set; }
}