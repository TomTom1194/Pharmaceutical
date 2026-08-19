namespace WebApp.Dtos;


public class ResumeResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ResumeResponseDto? Data { get; set; }
}
