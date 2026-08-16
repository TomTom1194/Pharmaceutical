using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;
using Pharmaceutical.Models;

namespace Pharmaceutical.Services;

public class ContentPageService : IContentPageService
{
    private readonly PharmaceuticalDbContext _db;

    public ContentPageService(PharmaceuticalDbContext db) => _db = db;

    public async Task<List<ContentPageDto>> GetAll() =>
        await _db.ContentPages
            .OrderBy(p => p.PageId)
            .Select(p => ToDto(p))
            .ToListAsync();

    public async Task<ContentPageDto?> GetBySlug(string slug)
    {
        var p = await _db.ContentPages.FirstOrDefaultAsync(x => x.Slug == slug);
        return p is null ? null : ToDto(p);
    }

    public async Task<ContentPageDto?> GetById(int id)
    {
        var p = await _db.ContentPages.FindAsync(id);
        return p is null ? null : ToDto(p);
    }

    public async Task<bool> Update(int id, SaveContentPageDto dto, int? adminUserId)
    {
        var entity = await _db.ContentPages.FindAsync(id);
        if (entity is null) return false;

        entity.Title = dto.Title;
        entity.Body = dto.Body;
        entity.BannerImageUrl = dto.BannerImageUrl;
        entity.Status = dto.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = adminUserId;

        await _db.SaveChangesAsync();
        return true;
    }

    private static ContentPageDto ToDto(ContentPage p) => new()
    {
        PageId = p.PageId,
        Slug = p.Slug,
        Title = p.Title,
        Body = p.Body,
        BannerImageUrl = p.BannerImageUrl,
        Status = p.Status,
        UpdatedAt = p.UpdatedAt
    };
}
