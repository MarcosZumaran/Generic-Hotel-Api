using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class TiposHabitacionService : ITiposHabitacionService
{
    private readonly HotelDbContext _db;
    private readonly TiposHabitacionMapper _mapper;

    public TiposHabitacionService(HotelDbContext db, TiposHabitacionMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TiposHabitacionResponseDto>> GetAllAsync()
    {
        var entities = await _db.TiposHabitacions.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<TiposHabitacionResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.TiposHabitacions.FindAsync(id);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<TiposHabitacionResponseDto> CreateAsync(TiposHabitacionCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.TiposHabitacions.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, TiposHabitacionUpdateDto dto)
    {
        var entity = await _db.TiposHabitacions.FindAsync(id);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.TiposHabitacions.FindAsync(id);
        if (entity is null) return false;
        _db.TiposHabitacions.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}