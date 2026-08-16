namespace WebApp.Dtos;

public class RegisterResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string? Message { get; set; }
}
