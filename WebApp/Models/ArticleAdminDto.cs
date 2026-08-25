using Microsoft.AspNetCore.Http;
namespace WebApp.Models;

public class ArticleAdminDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public string? Thumbnail { get; set; }
    public IFormFile? ThumbnailFile { get; set; }
    public string AuthorName { get; set; } = null!;
    public DateTime? PublishedAt { get; set; }
    public string Status { get; set; } = null!;
    public bool IsEditorPick { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SaveArticleViewModel
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public string? Thumbnail { get; set; }
    public IFormFile? ThumbnailFile { get; set; }
    public string AuthorName { get; set; } = "XYZ Pharma";
    public DateTime? PublishedAt { get; set; }
    public string Status { get; set; } = "Draft";
    public bool IsEditorPick { get; set; }
}


