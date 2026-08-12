using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EduAssign.API.Services;
using System.Security.Claims;

namespace EduAssign.API.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("my")]
[Authorize(Roles = "teacher")]
public async Task<IActionResult> GetMyStudents()
{
    try
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        Console.WriteLine($"=================================");
        Console.WriteLine($"Teacher ID: {teacherId}");
        Console.WriteLine($"=================================");

        if (string.IsNullOrEmpty(teacherId))
            return Unauthorized(new { message = "Teacher ID missing" });

        var students = await _studentService
            .GetStudentsForTeacherAsync(teacherId);

        Console.WriteLine($"Students found: {students.Count}");

        foreach (var student in students)
        {
            Console.WriteLine(
                $"Student: {student.Name} | Class: {student.Class} | Role: {student.Role}"
            );
        }

        return Ok(students);
    }
    catch (Exception ex)
    {
        Console.WriteLine("=================================");
        Console.WriteLine("STUDENT API ERROR:");
        Console.WriteLine(ex.ToString());
        Console.WriteLine("=================================");

        return StatusCode(500, new
        {
            message = ex.Message,
            detail = ex.ToString()
        });
    }
}
}