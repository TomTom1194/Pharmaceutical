namespace Pharmaceutical.Dtos;

public class ArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public string? Thumbnail { get; set; }
    public string AuthorName { get; set; } = null!;
    public DateTime? PublishedAt { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsEditorPick { get; set; }
}

public class SaveArticleDto
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public string? Thumbnail { get; set; }
    public string AuthorName { get; set; } = "XYZ Pharma";
    public DateTime? PublishedAt { get; set; }
    public string Status { get; set; } = "Draft";
    public bool IsEditorPick { get; set; }
}


