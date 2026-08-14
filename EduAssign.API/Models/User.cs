using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models;

[BsonIgnoreExtraElements] // Added to prevent crashes from extra fields in MongoDB
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = "Student";

    [BsonElement("subject")]
    public string? Subject { get; set; }

    [BsonElement("primarySubject")]
    public string? PrimarySubject { get; set; }

    [BsonElement("class")]
    public string? Class { get; set; }

    [BsonElement("isBlocked")]
    public bool IsBlocked { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}