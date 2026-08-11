using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("InterviewInvitation")]
public class InterviewInvitation
{
    [Key]
    [Column("invitation_id")]
    public int InvitationId { get; set; }

    [Column("candidate_id")]
    public int? CandidateId { get; set; }

    [Column("sent_by")]
    public int? SentBy { get; set; }

    [MaxLength(255)]
    [Column("subject")]
    public string? Subject { get; set; }

    [Column("body")]
    public string? Body { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string? Status { get; set; }

    [Column("sent_at")]
    public DateTime? SentAt { get; set; }
}