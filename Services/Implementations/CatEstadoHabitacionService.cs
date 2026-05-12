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
    private readonly EstadoHabitacionMapper _mapper;

    public CatEstadoHabitacionService(HotelDbContext db, EstadoHabitacionMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CatEstadoHabitacionResponseDto>> GetAllAsync()
    {
        var entities = await _db.EstadosHabitacion.AsNoTracking().ToListAsync();
        return entities.Select(_mapper.ToResponse);
    }

    public async Task<CatEstadoHabitacionResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.EstadosHabitacion.FindAsync(id);
        return entity is not null ? _mapper.ToResponse(entity) : null;
    }

    public async Task<CatEstadoHabitacionResponseDto> CreateAsync(CatEstadoHabitacionCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        _db.EstadosHabitacion.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, CatEstadoHabitacionUpdateDto dto)
    {
        var entity = await _db.EstadosHabitacion.FindAsync(id);
        if (entity is null) return false;
        _mapper.UpdateFromDto(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.EstadosHabitacion.FindAsync(id);
        if (entity is null) return false;
        _db.EstadosHabitacion.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}