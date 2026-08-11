using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("WorkExperience")]
public class WorkExperience
{
    [Key]
    [Column("experience_id")]
    public int ExperienceId { get; set; }

    [Column("candidate_id")]
    public int? CandidateId { get; set; }

    [MaxLength(255)]
    [Column("employer")]
    public string? Employer { get; set; }

    [MaxLength(255)]
    [Column("title")]
    public string? Title { get; set; }

    [Column("start_date")]
    public DateOnly? StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("description")]
    public string? Description { get; set; }
}