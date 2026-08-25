namespace WebApp.Dtos;


public class ApplicationDetailViewModel
{
    public ApplicationDto Application { get; set; } = null!;
    public CandidateProfileDto? Profile { get; set; }
    public ResumeResponseDto? Resume { get; set; }
}
