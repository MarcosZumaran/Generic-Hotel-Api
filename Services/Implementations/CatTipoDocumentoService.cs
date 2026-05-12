using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class CatTipoDocumentoService : ICatTipoDocumentoService
{
    private readonly HotelDbContext _db;
    private readonly TipoDocumentoMapper _mapper;

    public CatTipoDocumentoService(HotelDbContext db, TipoDocumentoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatTipoDocumentoResponseDto>> GetAllAsync()
    {
        var entities = await _db.TiposDocumento.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatTipoDocumentoResponseDto?> GetByIdAsync(string codigo)
    {
        var entity = await _db.TiposDocumento.FindAsync(codigo);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatTipoDocumentoResponseDto> CreateAsync(CatTipoDocumentoCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.TiposDocumento.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(string codigo, CatTipoDocumentoUpdateDto dto)
    {
        var entity = await _db.TiposDocumento.FindAsync(codigo);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string codigo)
    {
        var entity = await _db.TiposDocumento.FindAsync(codigo);
        if (entity is null) return false;
        _db.TiposDocumento.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}