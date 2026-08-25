namespace WebApp.Dtos;


public class ResumeResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ResumeResponseDto? Data { get; set; }
}


public class ResumeDownloadResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public byte[]? Content { get; set; }
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
}
