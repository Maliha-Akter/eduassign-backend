using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models
{
    public class Subject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string CourseId { get; set; } = string.Empty;
    }
}