using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;


[Table("ApplicationLog")]
public class ApplicationLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; }

    [Column("application_id")]
    public int ApplicationId { get; set; }

    [MaxLength(255)]
    [Column("full_name")]
    public string? FullName { get; set; }

    [MaxLength(50)]
    [Column("phone")]
    public string? Phone { get; set; }

    [MaxLength(255)]
    [Column("address")]
    public string? Address { get; set; }

    [Column("summary")]
    public string? Summary { get; set; }

    
    [MaxLength(255)]
    [Column("profile_image")]
    public string? ProfileImage { get; set; }

    
    [Column("educations_json")]
    public string? EducationsJson { get; set; }

    [Column("work_experiences_json")]
    public string? WorkExperiencesJson { get; set; }

    
    [MaxLength(255)]
    [Column("resume_original_name")]
    public string? ResumeOriginalName { get; set; }

    [MaxLength(255)]
    [Column("resume_storage_key")]
    public string? ResumeStorageKey { get; set; }

    [MaxLength(100)]
    [Column("resume_mime_type")]
    public string? ResumeMimeType { get; set; }

    [Column("resume_size")]
    public int? ResumeSize { get; set; }

    [Column("resume_uploaded_at")]
    public DateTime? ResumeUploadedAt { get; set; }

    [Column("logged_at")]
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ApplicationId))]
    public Application? Application { get; set; }
}

public class EducationSnapshotDto
{
    public string? Institution { get; set; }
    public string? Qualification { get; set; }
    public string? Field { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class WorkExperienceSnapshotDto
{
    public string? Employer { get; set; }
    public string? Title { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }
}
