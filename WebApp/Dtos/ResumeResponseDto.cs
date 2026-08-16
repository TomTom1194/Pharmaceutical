namespace WebApp.Dtos;

public class ResumeResponseDto
{
    public int ResumeId { get; set; }
    public string OriginalName { get; set; }
    public string? MimeType { get; set; }
    public int? Size { get; set; }
    public DateTime? UploadedAt { get; set; }
    public bool IsCurrent { get; set; }
}
