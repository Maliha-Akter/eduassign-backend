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

        [BsonElement("TeacherId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TeacherId { get; set; } = null!;

        [BsonIgnore]
        public string? TeacherName { get; set; }

        // 👈 NEW: Added TeacherEmail 
        [BsonIgnore]
        public string? TeacherEmail { get; set; }

        [BsonElement("ClassId")]
        public string ClassId { get; set; } = null!;

        [BsonElement("SubjectId")]
        public string SubjectId { get; set; } = null!;

        [BsonElement("Title")]
        public string Title { get; set; } = null!;
        
        [BsonElement("Description")]
        public string Description { get; set; } = null!;
        
        [BsonElement("Deadline")]
        public DateTime Deadline { get; set; }
        
        [BsonElement("MaximumMarks")]
        public int MaximumMarks { get; set; }
        
        [BsonElement("Status")]
        public string Status { get; set; } = "Draft";
        
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}