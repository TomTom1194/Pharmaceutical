namespace WebApp.Dtos;

public class CandidateProfileDto
{
    public int CandidateId { get; set; }
    public string Email { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Summary { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<EducationItemDto> Educations { get; set; } = new();
    public List<WorkExperienceItemDto> WorkExperiences { get; set; } = new();
}

public class EducationItemDto
{
    public int EducationId { get; set; }
    public string? Institution { get; set; }
    public string? Qualification { get; set; }
    public string? Field { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class WorkExperienceItemDto
{
    public int ExperienceId { get; set; }
    public string? Employer { get; set; }
    public string? Title { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }
}
