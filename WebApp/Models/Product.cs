using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Product
{
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Enter product name")]
    [MaxLength(255)]
    public string ModelName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Summary { get; set; }

    public string? Description { get; set; }

    [MaxLength(100)]
    public string? OutputLabel { get; set; }

    public bool? IsPublished { get; set; }

    public List<ImageProduct> Images { get; set; } = new();
}
