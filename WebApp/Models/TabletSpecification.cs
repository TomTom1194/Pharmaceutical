using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class TabletSpecification
{
    [Required(ErrorMessage = "Select a product")]
    public int ProductId { get; set; }

    [MaxLength(100)]
    public string? ModelNumber { get; set; }

    public int? Dies { get; set; }

    public decimal? MaxPressure { get; set; }

    public decimal? MaxDiameterMm { get; set; }

    public decimal? MaxDepthFillMm { get; set; }

    public decimal? ProductionCapacity { get; set; }

    [MaxLength(100)]
    public string? MachineSize { get; set; }

    public decimal? NetWeightKg { get; set; }
}
