namespace WebApp.Dtos;

public class AdminCandidateListItemDto
{
    public int CandidateId { get; set; }
    public string Email { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool HasResume { get; set; }
}

public class AdminCandidateDetailDto
{
    public int CandidateId { get; set; }
    public string Email { get; set; }
    public string? Status { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Summary { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<EducationItemDto> Educations { get; set; } = new();
    public List<WorkExperienceItemDto> WorkExperiences { get; set; } = new();
    public ResumeResponseDto? Resume { get; set; }
    public List<InterviewInvitationDto> Invitations { get; set; } = new();

    
    public int? ApplicationId { get; set; }
    public int? PositionId { get; set; }
    public string? PositionTitle { get; set; }
    public string? Department { get; set; }
    public DateTime? AppliedDate { get; set; }
    public string? ApplicationStatus { get; set; }
}

public class InterviewInvitationDto
{
    public int InvitationId { get; set; }
    public int? CandidateId { get; set; }
    public string? Subject { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? SentAt { get; set; }
}



public class SendInterviewInvitationRequestDto
{
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? Type { get; set; }
}

public class AdminCandidatesResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<AdminCandidateListItemDto> Data { get; set; } = new();
}

public class AdminCandidateDetailResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public AdminCandidateDetailDto? Data { get; set; }
}

public class InterviewInvitationResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public InterviewInvitationDto? Data { get; set; }
}


public class AdminPositionSummaryDto
{
    public int PositionId { get; set; }
    public string Title { get; set; }
    public string Department { get; set; }
    public int ApplicationCount { get; set; }
}


public class AdminPositionApplicationItemDto
{
    public int ApplicationId { get; set; }
    public int CandidateId { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string? AccountStatus { get; set; }
    public DateTime AppliedDate { get; set; }
    public string ApplicationStatus { get; set; }
    public bool HasResume { get; set; }
    public int PositionId { get; set; }
    public string? PositionTitle { get; set; }
}

public class AdminPositionsResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<AdminPositionSummaryDto> Data { get; set; } = new();
}

public class AdminPositionApplicationsResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<AdminPositionApplicationItemDto> Data { get; set; } = new();
}
