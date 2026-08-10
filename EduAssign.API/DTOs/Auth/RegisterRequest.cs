namespace EduAssign.API.DTOs.Auth;

public class RegisterRequest
{
public string Name { get; set; } = string.Empty;

public string Email { get; set; } = string.Empty;

public string Password { get; set; } = string.Empty;

public string Role { get; set; } = "Student";

public string? Subject { get; set; }

public string? Class { get; set; }

}