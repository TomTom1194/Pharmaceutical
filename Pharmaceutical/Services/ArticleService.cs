using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;
using Pharmaceutical.Models;
namespace Pharmaceutical.Services;
public class ArticleService : IArticleService
{
    private readonly PharmaceuticalDbContext _db;
    public ArticleService(PharmaceuticalDbContext db) => _db = db;
        public async Task<List<ArticleDto>> GetEditorsPicks() =>
        await _db.Articles
            .Where(a => a.Status == "Published" && a.IsEditorPick)
            .OrderByDescending(a => a.PublishedAt)
            .Take(5)
            .Select(a => ToDto(a))
            .ToListAsync();
    public async Task<List<ArticleDto>> GetPublished(string? search) =>
        await _db.Articles
            .Where(a => a.Status == "Published" &&
                        (search == null || a.Title.Contains(search) || (a.Summary != null && a.Summary.Contains(search))))
            .OrderByDescending(a => a.PublishedAt)
            .Select(a => ToDto(a))
            .ToListAsync();
        public async Task<List<ArticleDto>> GetAll(string? search, string? sort)
    {
        var query = _db.Articles.Where(a => search == null || a.Title.Contains(search));
        if (sort == "oldest")
            query = query.OrderBy(a => a.CreatedAt);
        else
            query = query.OrderByDescending(a => a.CreatedAt);
        return await query.Select(a => ToDto(a)).ToListAsync();
    }
    public async Task<ArticleDto?> GetBySlug(string slug)
    {
        var a = await _db.Articles.FirstOrDefaultAsync(x => x.Slug == slug);
        return a is null ? null : ToDto(a);
    }
    public async Task<ArticleDto?> GetById(int id)
    {
        var a = await _db.Articles.FindAsync(id);
        return a is null ? null : ToDto(a);
    }
    public async Task<ArticleDto> Create(SaveArticleDto dto)
    {
        var article = new Article
        {
            Title       = dto.Title,
            Slug        = dto.Slug,
            Summary     = dto.Summary,
            Content     = dto.Content,
            Thumbnail   = dto.Thumbnail,
            AuthorName  = dto.AuthorName,
            PublishedAt = dto.PublishedAt,
            Status      = dto.Status,
            IsEditorPick = dto.IsEditorPick,
            CreatedAt   = DateTime.UtcNow
        };
        _db.Articles.Add(article);
        await _db.SaveChangesAsync();
        return ToDto(article);
    }
    public async Task<bool> Update(int id, SaveArticleDto dto)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return false;
        article.Title       = dto.Title;
        article.Slug        = dto.Slug;
        article.Summary     = dto.Summary;
        article.Content     = dto.Content;
        article.Thumbnail   = dto.Thumbnail;
        article.AuthorName  = dto.AuthorName;
        article.PublishedAt = dto.PublishedAt;
        article.Status      = dto.Status;
        article.IsEditorPick = dto.IsEditorPick;
        article.UpdatedAt   = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
        public async Task<bool> UpdateEditorsPicks(List<int> ids)
    {
        var allPicks = await _db.Articles.Where(a => a.IsEditorPick).ToListAsync();
        foreach (var a in allPicks) a.IsEditorPick = false;
        var newPicks = await _db.Articles.Where(a => ids.Contains(a.Id)).ToListAsync();
        foreach (var a in newPicks) a.IsEditorPick = true;
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> Delete(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null) return false;
        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
        return true;
    }
    private static ArticleDto ToDto(Article a) => new()
    {
        Id          = a.Id,
        Title       = a.Title,
        Slug        = a.Slug,
        Summary     = a.Summary,
        Content     = a.Content,
        Thumbnail   = a.Thumbnail,
        AuthorName  = a.AuthorName,
        PublishedAt = a.PublishedAt,
        Status      = a.Status,
        IsEditorPick = a.IsEditorPick,
        CreatedAt   = a.CreatedAt,
        UpdatedAt   = a.UpdatedAt
    };
}
