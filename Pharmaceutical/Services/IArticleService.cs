using Pharmaceutical.Dtos;

namespace Pharmaceutical.Services;

public interface IArticleService
{
    Task<List<ArticleDto>> GetEditorsPicks();
    Task<List<ArticleDto>> GetPublished(string? search);
    Task<List<ArticleDto>> GetAll(string? search, string? sort);
    Task<ArticleDto?> GetBySlug(string slug);
    Task<ArticleDto?> GetById(int id);
    Task<ArticleDto> Create(SaveArticleDto dto);
    Task<bool> Update(int id, SaveArticleDto dto);
    Task<bool> Delete(int id);
    Task<bool> UpdateEditorsPicks(List<int> ids);
}



