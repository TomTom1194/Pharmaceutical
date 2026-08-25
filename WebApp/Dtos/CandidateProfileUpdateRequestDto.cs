using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApp.Dtos;


public class CandidateProfileUpdateRequestDto
{
    [Required(ErrorMessage = "Input Full Name")]
    [MaxLength(255)]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Input Phone")]
    [Phone(ErrorMessage = "Phone number is not valid")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Input Address")]
    [MaxLength(255)]
    public string? Address { get; set; }

    public string? Summary { get; set; }

    public List<EducationInputDto> Educations { get; set; } = new();
    public List<WorkExperienceInputDto> WorkExperiences { get; set; } = new();

    // Not decorated with [Required]: whether it's required depends on
    // whether the candidate already has an image on file, which can only
    // be checked against the database. That check is done manually in
    // CandidateController.UpdateResume (POST) instead.
    public IFormFile? ProfileImageFile { get; set; }

    
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
