namespace WebApp.Dtos;

// Returned by GET api/candidate/applications/{id}/detail: the candidate's
// profile/education/work-experience/resume as they were AT THE MOMENT this
// application was submitted (ApplicationLog), not the live profile. Extends
// CandidateProfileDto with the resume so it can be assigned directly to
// ApplicationDetailViewModel.Profile.
public class ApplicationSnapshotDto : CandidateProfileDto
{
    public ResumeResponseDto? Resume { get; set; }
}

public class ApplicationSnapshotResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ApplicationSnapshotDto? Data { get; set; }
}
