using Pharmaceutical.Dtos;

namespace Pharmaceutical.Services;

public interface IContentPageService
{
    Task<List<ContentPageDto>> GetAll();
    Task<ContentPageDto?> GetBySlug(string slug);
    Task<ContentPageDto?> GetById(int id);
    Task<bool> Update(int id, SaveContentPageDto dto, int? adminUserId);
}
