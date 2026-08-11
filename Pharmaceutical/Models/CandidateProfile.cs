using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("CandidateProfile")]
public class CandidateProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]   
    [Column("candidate_id")]
    public int CandidateId { get; set; }

    [MaxLength(255)]
    [Column("full_name")]
    public string? FullName { get; set; }

    [MaxLength(20)]
    [Column("phone")]
    public string? Phone { get; set; }

    [MaxLength(255)]
    [Column("address")]
    public string? Address { get; set; }

    [Column("summary")]
    public string? Summary { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
}