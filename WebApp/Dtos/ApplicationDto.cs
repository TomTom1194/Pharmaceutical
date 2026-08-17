namespace WebApp.Dtos;

public class ApplicationDto
{
    public int ApplicationId { get; set; }
    public int PositionId { get; set; }
    public string PositionTitle { get; set; } = null!;
    public string Department { get; set; } = null!;
    public DateTime AppliedDate { get; set; }
    public string Status { get; set; } = null!;
}

// Wraps the outcome of an applications lookup so the controller can surface
// the real reason for failure instead of a generic message.
public class ApplicationListResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<ApplicationDto> Data { get; set; } = new();
}
