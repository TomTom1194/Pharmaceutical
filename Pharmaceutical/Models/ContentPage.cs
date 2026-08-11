using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("ContentPage")]
public class ContentPage
{
    [Key]
    [Column("page_id")]
    public int PageId { get; set; }

    [Required, MaxLength(100)]
    [Column("slug")]
    public string Slug { get; set; } = null!;

    [MaxLength(255)]
    [Column("title")]
    public string? Title { get; set; }

    [Column("body")]
    public string? Body { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string? Status { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}