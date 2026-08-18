using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class QuoteRequest
{
    public int QuoteId { get; set; }

    [Required(ErrorMessage = "Enter full name")]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? CompanyName { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Enter email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? Comments { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public int? HandledBy { get; set; }
}
