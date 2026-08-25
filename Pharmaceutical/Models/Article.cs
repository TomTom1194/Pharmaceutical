using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("Article")]
public class Article
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = null!;

    [Required, MaxLength(255)]
    [Column("slug")]
    public string Slug { get; set; } = null!;

    [MaxLength(500)]
    [Column("summary")]
    public string? Summary { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!;

    [MaxLength(500)]
    [Column("thumbnail")]
    public string? Thumbnail { get; set; }

    [MaxLength(100)]
    [Column("author_name")]
    public string AuthorName { get; set; } = "XYZ Pharma";

    [Column("published_at")]
    public DateTime? PublishedAt { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Draft";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("IsEditorPick")]
    public bool IsEditorPick { get; set; }
}


