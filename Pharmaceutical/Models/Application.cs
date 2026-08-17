using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("Application")]
public class Application
{
    [Key]
    [Column("application_id")]
    public int ApplicationId { get; set; }

    [Required]
    [Column("candidate_id")]
    public int CandidateId { get; set; }

    [Required]
    [Column("PositionId")]
    public int PositionId { get; set; }

    [Column("AppliedDate")]
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(50)]
    [Column("Status")]
    public string Status { get; set; } = "Applied";

    [ForeignKey(nameof(CandidateId))]
    public CandidateProfile? Candidate { get; set; }

    [ForeignKey(nameof(PositionId))]
    public Position? Position { get; set; }
}
