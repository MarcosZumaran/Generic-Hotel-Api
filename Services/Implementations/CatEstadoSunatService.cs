using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class CatEstadoSunatService : ICatEstadoSunatService
{
    private readonly HotelDbContext _db;
    private readonly CatEstadoSunatMapper _mapper;

    public CatEstadoSunatService(HotelDbContext db, CatEstadoSunatMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatEstadoSunatResponseDto>> GetAllAsync()
    {
        var entities = await _db.CatEstadoSunats.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatEstadoSunatResponseDto?> GetByIdAsync(int codigo)
    {
        var entity = await _db.CatEstadoSunats.FindAsync(codigo);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatEstadoSunatResponseDto> CreateAsync(CatEstadoSunatCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.CatEstadoSunats.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int codigo, CatEstadoSunatUpdateDto dto)
    {
        var entity = await _db.CatEstadoSunats.FindAsync(codigo);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int codigo)
    {
        var entity = await _db.CatEstadoSunats.FindAsync(codigo);
        if (entity is null) return false;
        _db.CatEstadoSunats.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}