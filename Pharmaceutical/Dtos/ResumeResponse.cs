namespace Pharmaceutical.Dtos;

public class ResumeResponse
{
    public int ResumeId { get; set; }
    public string OriginalName { get; set; } = null!;
    public string? MimeType { get; set; }
    public int? Size { get; set; }
    public DateTime? UploadedAt { get; set; }
    public bool IsCurrent { get; set; }
}
