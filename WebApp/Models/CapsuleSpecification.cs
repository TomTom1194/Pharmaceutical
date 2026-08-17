using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class CapsuleSpecification
{
    [Required(ErrorMessage = "Select a product")]
    public int ProductId { get; set; }

    [MaxLength(100)]
    public string? Output { get; set; }

    public decimal? CapsuleSizeMm { get; set; }

    [MaxLength(100)]
    public string? MachineDimension { get; set; }

    public decimal? ShippingWeightKg { get; set; }
}
