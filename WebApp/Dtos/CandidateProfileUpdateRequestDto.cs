using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApp.Dtos;

// Bound to the "Update Resume" form: personal info plus any number of
// education / work-experience entries the candidate fills in.
public class CandidateProfileUpdateRequestDto
{
    [Required(ErrorMessage = "Input Full Name")]
    [MaxLength(255)]
    public string? FullName { get; set; }

    [Phone(ErrorMessage = "Phone number is not valid")]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    public string? Summary { get; set; }

    public List<EducationInputDto> Educations { get; set; } = new();
    public List<WorkExperienceInputDto> WorkExperiences { get; set; } = new();

    // Optional: when the candidate picks a new photo, it is saved together
    // with the rest of the form on Save (no separate upload button/request).
    public IFormFile? ProfileImageFile { get; set; }

    // Optional: when the candidate picks a CV file (PDF/DOC/DOCX), it replaces
    // their current resume file on Save, together with the rest of the form.
    public IFormFile? ResumeFile { get; set; }
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
