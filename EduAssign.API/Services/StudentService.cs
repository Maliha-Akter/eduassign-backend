using EduAssign.API.Models;
using MongoDB.Driver;

namespace EduAssign.API.Services;

public class StudentService : IStudentService
{
    private readonly IMongoCollection<AppUser> _users;
    private readonly IMongoCollection<Assignment> _assignments;

    public StudentService(IMongoDatabase database)
    {
        _users = database.GetCollection<AppUser>("user");
        _assignments = database.GetCollection<Assignment>("assignments");
    }

    public async Task<List<AppUser>> GetStudentsForTeacherAsync(string teacherId)
{
    Console.WriteLine($"StudentService TeacherId: {teacherId}");

    var teacherAssignments = await _assignments
        .Find(a => a.TeacherId == teacherId)
        .ToListAsync();

    Console.WriteLine(
        $"Teacher assignments found: {teacherAssignments.Count}"
    );

    foreach (var assignment in teacherAssignments)
    {
        Console.WriteLine(
            $"Assignment: {assignment.Title} | ClassId: {assignment.ClassId}"
        );
    }

    var teacherClasses = teacherAssignments
        .Select(a => a.ClassId?.Replace("class_", "") ?? "")
        .Where(id => !string.IsNullOrEmpty(id))
        .Distinct()
        .ToList();

    Console.WriteLine(
        $"Teacher classes: {string.Join(", ", teacherClasses)}"
    );

    if (!teacherClasses.Any())
    {
        Console.WriteLine("No classes found.");
        return new List<AppUser>();
    }

    var filter = Builders<AppUser>.Filter.And(
        Builders<AppUser>.Filter.Eq(u => u.Role, "student"),
        Builders<AppUser>.Filter.In(u => u.Class, teacherClasses)
    );

    var students = await _users
        .Find(filter)
        .ToListAsync();

    Console.WriteLine(
        $"Students matching classes: {students.Count}"
    );

    return students;
}
}