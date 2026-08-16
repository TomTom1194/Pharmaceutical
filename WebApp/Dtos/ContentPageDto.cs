namespace WebApp.Dtos;

public class ContentPageDto
{
    public int PageId { get; set; }
    public string Slug { get; set; } = null!;
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? BannerImageUrl { get; set; }
    public string? Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
