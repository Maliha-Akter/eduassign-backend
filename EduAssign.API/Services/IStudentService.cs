using EduAssign.API.Models;

namespace EduAssign.API.Services;

public interface IStudentService
{
    Task<List<AppUser>> GetStudentsForTeacherAsync(string teacherId);
}