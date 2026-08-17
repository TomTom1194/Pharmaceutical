namespace WebApp.Dtos;

// Wraps the outcome of a resume upload/lookup call so the controller can
// surface the real reason for failure instead of a generic message.
public class ResumeResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ResumeResponseDto? Data { get; set; }
}
