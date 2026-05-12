using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class CatTipoComprobanteService : ICatTipoComprobanteService
{
    private readonly HotelDbContext _db;
    private readonly TipoComprobanteMapper _mapper;

    public CatTipoComprobanteService(HotelDbContext db, TipoComprobanteMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatTipoComprobanteResponseDto>> GetAllAsync()
    {
        var entities = await _db.TiposComprobante.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatTipoComprobanteResponseDto?> GetByIdAsync(string codigo)
    {
        var entity = await _db.TiposComprobante.FindAsync(codigo);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatTipoComprobanteResponseDto> CreateAsync(CatTipoComprobanteCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.TiposComprobante.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(string codigo, CatTipoComprobanteUpdateDto dto)
    {
        var entity = await _db.TiposComprobante.FindAsync(codigo);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string codigo)
    {
        var entity = await _db.TiposComprobante.FindAsync(codigo);
        if (entity is null) return false;
        _db.TiposComprobante.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}