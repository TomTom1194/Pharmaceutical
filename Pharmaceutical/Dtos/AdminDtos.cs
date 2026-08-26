using System.ComponentModel.DataAnnotations;

namespace Pharmaceutical.Dtos;

public class AdminCandidateListItem
{
    public int CandidateId { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool HasResume { get; set; }
}

public class AdminCandidateDetailResponse
{
    public int CandidateId { get; set; }
    public string Email { get; set; } = null!;
    public string? Status { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Summary { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool HasProfileImage { get; set; }
    public List<EducationItemDto> Educations { get; set; } = new();
    public List<WorkExperienceItemDto> WorkExperiences { get; set; } = new();
    public ResumeResponse? Resume { get; set; }
    public List<InterviewInvitationResponse> Invitations { get; set; } = new();

    // Populated only when the detail is requested with a positionId (i.e.
    // viewed from a specific position's applications list) — the candidate's
    // application to that one position.
    public int? ApplicationId { get; set; }
    public int? PositionId { get; set; }
    public string? PositionTitle { get; set; }
    public string? Department { get; set; }
    public DateTime? AppliedDate { get; set; }
    public string? ApplicationStatus { get; set; }
}

// Admin can leave Subject/Body blank to use the default recruitment template.
// Type selects which tab this was sent from — "Interview" (default),
// "Offer" or "Decline" — which decides the default template used when
// Subject/Body are blank and what the parent Application's Status becomes.
public class SendInterviewInvitationRequest
{
    [MaxLength(255)]
    public string? Subject { get; set; }

    public string? Body { get; set; }

    public string? Type { get; set; }
}

public class InterviewInvitationResponse
{
    public int InvitationId { get; set; }
    public int? CandidateId { get; set; }
    public string? Subject { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? SentAt { get; set; }
}

// Used by the Admin Portal "Applications" page: one button per open position.
public class AdminPositionSummaryResponse
{
    public int PositionId { get; set; }
    public string Title { get; set; } = null!;
    public string Department { get; set; } = null!;
    public int ApplicationCount { get; set; }
}

// A single candidate's application to a given position, with enough profile
// info for the admin to triage before opening the full CV.
public class AdminPositionApplicationItem
{
    public int ApplicationId { get; set; }
    public int CandidateId { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? AccountStatus { get; set; }
    public DateTime AppliedDate { get; set; }
    public string ApplicationStatus { get; set; } = null!;
    public bool HasResume { get; set; }
    public int PositionId { get; set; }
    public string? PositionTitle { get; set; }
}
