namespace WebApp.Dtos;


public class CandidateProfileResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public CandidateProfileDto? Data { get; set; }
}
