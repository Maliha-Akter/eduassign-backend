using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models
{
    [BsonIgnoreExtraElements]
    public class TeacherAssignment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string TeacherId { get; set; } = string.Empty;

        public string AssignedClass { get; set; } = string.Empty;

        public string Section { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("PrimarySubject")]
        public string PrimarySubject { get; set; } = string.Empty;
    }
}