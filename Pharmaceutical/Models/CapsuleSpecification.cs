using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("CapsuleSpecification")]
public class CapsuleSpecification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("product_id")]
    public int ProductId { get; set; }

    [MaxLength(100)]
    [Column("output")]
    public string? Output { get; set; }

    [Column("capsule_size_mm", TypeName = "decimal(10,2)")]
    public decimal? CapsuleSizeMm { get; set; }

    [MaxLength(100)]
    [Column("machine_dimension")]
    public string? MachineDimension { get; set; }

    [Column("shipping_weight_kg", TypeName = "decimal(10,2)")]
    public decimal? ShippingWeightKg { get; set; }
}