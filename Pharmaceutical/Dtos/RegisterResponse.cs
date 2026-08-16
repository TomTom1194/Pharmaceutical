namespace Pharmaceutical.Dtos;

public class RegisterResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Message { get; set; } = "Registration successful";
}
