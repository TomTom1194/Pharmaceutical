using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;


[Table("ProductCategory")]
public class ProductCategory
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Required, MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }
}