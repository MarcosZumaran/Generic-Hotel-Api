using Microsoft.EntityFrameworkCore;
using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;
using LaRicaNoche.Api.Mappings;
using LaRicaNoche.Api.Services.Interfaces;

namespace LaRicaNoche.Api.Services.Implementations;

public class CatTipoDocumentoService : ICatTipoDocumentoService
{
    private readonly LaRicaNocheDbContext _db;
    private readonly CatTipoDocumentoMapper _mapper;

    public CatTipoDocumentoService(LaRicaNocheDbContext db, CatTipoDocumentoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatTipoDocumentoResponseDto>> GetAllAsync()
    {
        var entities = await _db.CatTipoDocumentos.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatTipoDocumentoResponseDto?> GetByIdAsync(string codigo)
    {
        var entity = await _db.CatTipoDocumentos.FindAsync(codigo);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatTipoDocumentoResponseDto> CreateAsync(CatTipoDocumentoCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.CatTipoDocumentos.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(string codigo, CatTipoDocumentoUpdateDto dto)
    {
        var entity = await _db.CatTipoDocumentos.FindAsync(codigo);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string codigo)
    {
        var entity = await _db.CatTipoDocumentos.FindAsync(codigo);
        if (entity is null) return false;
        _db.CatTipoDocumentos.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}