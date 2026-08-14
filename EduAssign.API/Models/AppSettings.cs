using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduAssign.API.Models
{
    public class AppSettings
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string ApplicationName { get; set; } = "EduAssign";
        public bool AllowStudentRegistration { get; set; } = true;
        public bool AllowTeacherRegistration { get; set; } = false;
        public int MaxFileSizeMB { get; set; } = 10;
    }
}