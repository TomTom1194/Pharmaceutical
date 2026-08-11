using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("Product")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Required, MaxLength(255)]
    [Column("model_name")]
    public string ModelName { get; set; } = null!;

    [MaxLength(255)]
    [Column("summary")]
    public string? Summary { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [MaxLength(100)]
    [Column("output_label")]
    public string? OutputLabel { get; set; }

    [Column("is_published")]
    public bool? IsPublished { get; set; }
}