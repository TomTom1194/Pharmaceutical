using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("LiquidFillingSpecification")]
public class LiquidFillingSpecification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("air_pressure", TypeName = "decimal(10,2)")]
    public decimal? AirPressure { get; set; }

    [Column("air_volume", TypeName = "decimal(10,2)")]
    public decimal? AirVolume { get; set; }

    [Column("filling_speed", TypeName = "decimal(10,2)")]
    public decimal? FillingSpeed { get; set; }

    [Column("filling_range_ml", TypeName = "decimal(10,2)")]
    public decimal? FillingRangeMl { get; set; }
}