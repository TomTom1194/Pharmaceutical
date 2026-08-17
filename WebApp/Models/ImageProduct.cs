namespace WebApp.Models;

public class ImageProduct
{
    public int ImageId { get; set; }

    public int? ProductId { get; set; }

    public string Url { get; set; } = string.Empty;

    public int? DisplayOrder { get; set; }

    public bool? IsThumbnail { get; set; }
}
