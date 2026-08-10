using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EduAssign.API.Models
{
    public class Assignment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string TeacherId { get; set; } = null!;

        // Remove [BsonRepresentation(BsonType.ObjectId)] so they accept strings like "class_123"
        public string ClassId { get; set; } = null!;

        public string SubjectId { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public int MaximumMarks { get; set; }
        public string Status { get; set; } = "Draft"; // "Draft" or "Published"
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}