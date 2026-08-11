using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("ResumeFile")]
public class ResumeFile
{
    [Key]
    [Column("resume_id")]
    public int ResumeId { get; set; }

    [Column("candidate_id")]
    public int? CandidateId { get; set; }

    [Required, MaxLength(255)]
    [Column("storage_key")]
    public string StorageKey { get; set; } = null!;

    [MaxLength(255)]
    [Column("original_name")]
    public string? OriginalName { get; set; }

    [MaxLength(100)]
    [Column("mime_type")]
    public string? MimeType { get; set; }

    [Column("size")]
    public int? Size { get; set; }

    [Column("uploaded_at")]
    public DateTime? UploadedAt { get; set; }

    [Column("is_current")]
    public bool? IsCurrent { get; set; }
}