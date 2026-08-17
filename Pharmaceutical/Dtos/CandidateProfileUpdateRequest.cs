using System.ComponentModel.DataAnnotations;

namespace Pharmaceutical.Dtos;

// Used by the "Create Resume" flow in the candidate portal: fills in personal
// info plus any number of education / work-experience entries in one shot.
public class CandidateProfileUpdateRequest
{
    [MaxLength(255)]
    public string? FullName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    public string? Summary { get; set; }

    public List<EducationInputDto> Educations { get; set; } = new();
    public List<WorkExperienceInputDto> WorkExperiences { get; set; } = new();
}

public class EducationInputDto
{
    [MaxLength(255)]
    public string? Institution { get; set; }

    [MaxLength(255)]
    public string? Qualification { get; set; }

    [MaxLength(255)]
    public string? Field { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class WorkExperienceInputDto
{
    [MaxLength(255)]
    public string? Employer { get; set; }

    [MaxLength(255)]
    public string? Title { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Description { get; set; }
}
