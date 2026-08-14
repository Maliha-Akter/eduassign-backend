using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System; // Added missing System using for DateTime

namespace EduAssign.API.Models;

// ADDED: This prevents your API from crashing if you ever manually add a new field 
// (like 'GradedBy') to the MongoDB document that isn't listed in this C# class.
[BsonIgnoreExtraElements] 
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

    // ADDED: Ensures MongoDB strictly handles this as UTC to prevent timezone bugs in React
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // ADDED: Ensures MongoDB strictly handles this as UTC
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdatedAt { get; set; }

    [BsonIgnore]
    public string? Subject { get; set; }
}