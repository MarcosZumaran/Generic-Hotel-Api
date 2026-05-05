using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class CatEstadoHabitacionService : ICatEstadoHabitacionService
{
    private readonly HotelDbContext _db;
    private readonly CatEstadoHabitacionMapper _mapper;

    public CatEstadoHabitacionService(HotelDbContext db, CatEstadoHabitacionMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatEstadoHabitacionResponseDto>> GetAllAsync()
    {
        var entities = await _db.CatEstadoHabitacions.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatEstadoHabitacionResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.CatEstadoHabitacions.FindAsync(id);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatEstadoHabitacionResponseDto> CreateAsync(CatEstadoHabitacionCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.CatEstadoHabitacions.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, CatEstadoHabitacionUpdateDto dto)
    {
        var entity = await _db.CatEstadoHabitacions.FindAsync(id);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.CatEstadoHabitacions.FindAsync(id);
        if (entity is null) return false;
        _db.CatEstadoHabitacions.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}