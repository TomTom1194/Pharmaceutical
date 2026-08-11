using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("EducationRecord")]
public class EducationRecord
{
    [Key]
    [Column("education_id")]
    public int EducationId { get; set; }

    [Column("candidate_id")]
    public int? CandidateId { get; set; }

    [MaxLength(255)]
    [Column("institution")]
    public string? Institution { get; set; }

    [MaxLength(255)]
    [Column("qualification")]
    public string? Qualification { get; set; }

    [MaxLength(255)]
    [Column("field")]
    public string? Field { get; set; }

    [Column("start_date")]
    public DateOnly? StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }
}