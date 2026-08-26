namespace WebApp.Dtos;

// Composed view model for the Candidate portal's "My Applications" -> "Detail"
// page: the applied position + status, plus a read-only snapshot of the
// candidate's profile (photo, contact info, CV file, education, work
// experience) at the time the page is viewed.
public class ApplicationDetailViewModel
{
    public ApplicationDto Application { get; set; } = null!;
    public CandidateProfileDto? Profile { get; set; }
    public ResumeResponseDto? Resume { get; set; }

    // Used to scope the profile photo link to this application's snapshot
    // (see CandidateController.ApplicationProfileImage) instead of the
    // candidate's current, possibly since-changed, live photo.
    public int ApplicationId { get; set; }
}
