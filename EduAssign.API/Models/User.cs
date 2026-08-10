using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models;

public class User
{
[BsonId]
[BsonRepresentation(BsonType.ObjectId)]
public string Id { get; set; } = string.Empty;

public string Name { get; set; } = string.Empty;

public string Email { get; set; } = string.Empty;

public string PasswordHash { get; set; } = string.Empty;

public string Role { get; set; } = "Student";

public string? Subject { get; set; }

public string? Class { get; set; }

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}