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

    // Ties this invitation to the specific Application it was sent for, so
    // that a candidate who applies to multiple positions doesn't have every
    // position's invitation history bleed into every other position's
    // CandidateDetail page. Nullable because older rows (sent before this
    // column existed) and any invitation sent without position context have
    // no application to attach to.
    [Column("application_id")]
    public int? ApplicationId { get; set; }

    [Column("sent_by")]
    public int? SentBy { get; set; }

    [MaxLength(255)]
    [Column("subject")]
    public string? Subject { get; set; }

    // Which kind of email this was: "Interview", "Offer" or "Decline". Drives
    // what the parent Application's Status gets set to when the send
    // succeeds. Nullable because rows recorded before this column existed
    // only ever represented interview invitations.
    [MaxLength(20)]
    [Column("type")]
    public string? Type { get; set; }

    [Column("body")]
    public string? Body { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string? Status { get; set; }

    [Column("sent_at")]
    public DateTime? SentAt { get; set; }
}