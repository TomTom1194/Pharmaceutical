using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("TabletSpecification")]
public class TabletSpecification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("product_id")]
    public int ProductId { get; set; }

    [MaxLength(100)]
    [Column("model_number")]
    public string? ModelNumber { get; set; }

    [Column("dies")]
    public int? Dies { get; set; }

    [Column("max_pressure", TypeName = "decimal(10,2)")]
    public decimal? MaxPressure { get; set; }

    [Column("max_diameter_mm", TypeName = "decimal(10,2)")]
    public decimal? MaxDiameterMm { get; set; }

    [Column("max_depth_fill_mm", TypeName = "decimal(10,2)")]
    public decimal? MaxDepthFillMm { get; set; }

    [Column("production_capacity", TypeName = "decimal(10,2)")]
    public decimal? ProductionCapacity { get; set; }

    [MaxLength(100)]
    [Column("machine_size")]
    public string? MachineSize { get; set; }

    [Column("net_weight_kg", TypeName = "decimal(10,2)")]
    public decimal? NetWeightKg { get; set; }
}