using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("QuoteRequest")]
public class QuoteRequest
{
    [Key]
    [Column("quote_id")]
    public int QuoteId { get; set; }

    [Required, MaxLength(255)]
    [Column("full_name")]
    public string FullName { get; set; } = null!;

    [MaxLength(255)]
    [Column("company_name")]
    public string? CompanyName { get; set; }

    [MaxLength(255)]
    [Column("address")]
    public string? Address { get; set; }

    [MaxLength(100)]
    [Column("city")]
    public string? City { get; set; }

    [MaxLength(100)]
    [Column("state")]
    public string? State { get; set; }

    [MaxLength(20)]
    [Column("postal_code")]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    [Column("country")]
    public string? Country { get; set; }

    [Required, MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [MaxLength(20)]
    [Column("phone")]
    public string? Phone { get; set; }

    [Column("comments")]
    public string? Comments { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string? Status { get; set; }

    [Column("submitted_at")]
    public DateTime? SubmittedAt { get; set; }

    [Column("handled_by")]
    public int? HandledBy { get; set; }
}