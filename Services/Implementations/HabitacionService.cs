using Microsoft.EntityFrameworkCore;
using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;
using LaRicaNoche.Api.Mappings;
using LaRicaNoche.Api.Services.Interfaces;
using LaRicaNoche.Api.Models;

namespace LaRicaNoche.Api.Services.Implementations;

public class HabitacionService : IHabitacionService
{
    private readonly LaRicaNocheDbContext _db;
    private readonly HabitacionMapper _mapper;

    public HabitacionService(LaRicaNocheDbContext db, HabitacionMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HabitacionResponseDto>> GetAllAsync()
    {
        var entities = await _db.Habitaciones
            .Include(h => h.IdTipoNavigation)
            .Include(h => h.IdEstadoNavigation)
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(MapToResponse);
    }

    public async Task<HabitacionResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Habitaciones
            .Include(h => h.IdTipoNavigation)
            .Include(h => h.IdEstadoNavigation)
            .FirstOrDefaultAsync(h => h.IdHabitacion == id);

        return entity is not null ? MapToResponse(entity) : null;
    }

    public async Task<HabitacionResponseDto> CreateAsync(HabitacionCreateDto dto, int? idUsuario)
    {
        bool existe = await _db.Habitaciones.AnyAsync(h => h.NumeroHabitacion == dto.NumeroHabitacion);
        if (existe)
            throw new InvalidOperationException("Ya existe una habitación con ese número.");

        var entity = _mapper.FromCreate(dto);
        entity.FechaUltimoCambio = DateTime.UtcNow;
        entity.UsuarioCambio = idUsuario;

        _db.Habitaciones.Add(entity);
        await _db.SaveChangesAsync();

        await _db.Entry(entity).Reference(h => h.IdTipoNavigation).LoadAsync();
        await _db.Entry(entity).Reference(h => h.IdEstadoNavigation).LoadAsync();

        return MapToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, HabitacionUpdateDto dto, int? idUsuario)
    {
        var entity = await _db.Habitaciones.FindAsync(id);
        if (entity is null) return false;

        _mapper.UpdateFromDto(dto, entity);
        entity.FechaUltimoCambio = DateTime.UtcNow;
        entity.UsuarioCambio = idUsuario;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Habitaciones.FindAsync(id);
        if (entity is null) return false;

        bool tieneEstanciaActiva = await _db.Estancias.AnyAsync(e => e.IdHabitacion == id && e.Estado == "Activa");
        if (tieneEstanciaActiva)
            throw new InvalidOperationException("No se puede eliminar una habitación con estancias activas.");

        _db.Habitaciones.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    // Método privado para mapear entidad a DTO
    private static HabitacionResponseDto MapToResponse(Habitacione h)
    {
        return new HabitacionResponseDto(
            h.IdHabitacion,
            h.NumeroHabitacion,
            h.Piso,
            h.Descripcion,
            h.IdTipo,
            h.IdTipoNavigation?.Nombre ?? "",
            h.PrecioNoche,
            h.IdEstado,
            h.IdEstadoNavigation?.Nombre ?? "",
            h.FechaUltimoCambio,
            h.UsuarioCambio
        );
    }
}