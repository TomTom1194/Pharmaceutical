using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class LiquidFillingSpecification
{
    [Required(ErrorMessage = "Select a product")]
    public int ProductId { get; set; }

    public decimal? AirPressure { get; set; }

    public decimal? AirVolume { get; set; }

    public decimal? FillingSpeed { get; set; }

    public decimal? FillingRangeMl { get; set; }
}
