using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;
using Pharmaceutical.Models;

namespace Pharmaceutical.Services;

public class QuoteService : IQuoteService
{
    private readonly PharmaceuticalDbContext _db;

    public QuoteService(PharmaceuticalDbContext db) => _db = db;

    public async Task<List<QuoteRequestDto>> GetAll() =>
        await _db.QuoteRequests
            .OrderByDescending(q => q.SubmittedAt)
            .Select(q => ToDto(q))
            .ToListAsync();

    public async Task<QuoteRequestDto?> GetById(int id)
    {
        var q = await _db.QuoteRequests.FindAsync(id);
        return q is null ? null : ToDto(q);
    }

    public async Task<QuoteRequestDto> Create(CreateQuoteDto dto)
    {
        var entity = new QuoteRequest
        {
            FullName = dto.FullName,
            CompanyName = dto.CompanyName,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            Email = dto.Email,
            Phone = dto.Phone,
            Comments = dto.Comments,
            Status = "Pending",
            SubmittedAt = DateTime.UtcNow
        };

        _db.QuoteRequests.Add(entity);
        await _db.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> UpdateStatus(int id, UpdateQuoteStatusDto dto)
    {
        var entity = await _db.QuoteRequests.FindAsync(id);
        if (entity is null) return false;

        entity.Status = dto.Status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _db.QuoteRequests.FindAsync(id);
        if (entity is null) return false;

        _db.QuoteRequests.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private static QuoteRequestDto ToDto(QuoteRequest q) => new()
    {
        QuoteId = q.QuoteId,
        FullName = q.FullName,
        CompanyName = q.CompanyName,
        Address = q.Address,
        City = q.City,
        State = q.State,
        PostalCode = q.PostalCode,
        Country = q.Country,
        Email = q.Email,
        Phone = q.Phone,
        Comments = q.Comments,
        Status = q.Status,
        SubmittedAt = q.SubmittedAt
    };
}
