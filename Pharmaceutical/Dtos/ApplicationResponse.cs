namespace Pharmaceutical.Dtos;

public class ApplicationResponse
{
    public int ApplicationId { get; set; }
    public int PositionId { get; set; }
    public string PositionTitle { get; set; } = null!;
    public string Department { get; set; } = null!;
    public DateTime AppliedDate { get; set; }
    public string Status { get; set; } = null!;
}
