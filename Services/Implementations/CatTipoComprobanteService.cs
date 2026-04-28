using Microsoft.EntityFrameworkCore;
using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;
using LaRicaNoche.Api.Mappings;
using LaRicaNoche.Api.Services.Interfaces;

namespace LaRicaNoche.Api.Services.Implementations;

public class CatTipoComprobanteService : ICatTipoComprobanteService
{
    private readonly LaRicaNocheDbContext _db;
    private readonly CatTipoComprobanteMapper _mapper;

    public CatTipoComprobanteService(LaRicaNocheDbContext db, CatTipoComprobanteMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatTipoComprobanteResponseDto>> GetAllAsync()
    {
        var entities = await _db.CatTipoComprobantes.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatTipoComprobanteResponseDto?> GetByIdAsync(string codigo)
    {
        var entity = await _db.CatTipoComprobantes.FindAsync(codigo);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatTipoComprobanteResponseDto> CreateAsync(CatTipoComprobanteCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.CatTipoComprobantes.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(string codigo, CatTipoComprobanteUpdateDto dto)
    {
        var entity = await _db.CatTipoComprobantes.FindAsync(codigo);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string codigo)
    {
        var entity = await _db.CatTipoComprobantes.FindAsync(codigo);
        if (entity is null) return false;
        _db.CatTipoComprobantes.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}