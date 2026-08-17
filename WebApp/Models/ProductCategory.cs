using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ProductCategory
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Enter category name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}
