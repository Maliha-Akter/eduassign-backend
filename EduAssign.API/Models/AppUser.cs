using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models;

[BsonIgnoreExtraElements]
public class AppUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = string.Empty;

    // Matches your MongoDB field: "class": "12"
    [BsonElement("class")] 
    public string Class { get; set; } = string.Empty; 
}