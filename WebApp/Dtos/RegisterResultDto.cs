namespace WebApp.Dtos;


public class RegisterResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public RegisterResponseDto? Data { get; set; }
}
