using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models
{

    [Table("ImageProduct")]
    public class ImageProduct
    {
        [Key]
        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Required, MaxLength(500)]
        [Column("url")]
        public string Url { get; set; } = null!;

        [Column("display_order")]
        public int? DisplayOrder { get; set; }

        [Column("is_thumbnail")]
        public bool? IsThumbnail { get; set; }
    }
}
