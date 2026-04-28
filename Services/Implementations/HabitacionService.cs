using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Request.Create;
using LaRicaNoche.Api.DTOs.Request.Update;
using LaRicaNoche.Api.DTOs.Response;
using LaRicaNoche.Api.Models;
using LaRicaNoche.Api.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LaRicaNoche.Api.Services.Implementations;

public class HabitacionService : IHabitacionService
{
    public readonly LaRicaNocheDbContext _db;
    public readonly ILuaService _lua;
    public HabitacionService(LaRicaNocheDbContext db, ILuaService lua)
    {
        _db = db;
        _lua = lua;
    }

    public async Task<List<HabitacionResponseDTO>> GetAllAsync()
    {
        return await _db.Habitaciones
            .Include(h => h.IdTipoNavigation)
            .Include(h => h.IdEstadoNavigation)
            .ProjectToType<HabitacionResponseDTO>()
            .ToListAsync();
    }

    public async Task<HabitacionResponseDTO?> GetByIdAsync(int id)
    {
        var entity = await _db.Habitaciones
            .Include(h => h.IdTipoNavigation)
            .Include(h => h.IdEstadoNavigation)
            .ProjectToType<HabitacionResponseDTO>()
            .FirstOrDefaultAsync(h => h.IdHabitacion == id);
        return entity?.Adapt<HabitacionResponseDTO>();
    }

    public async Task<HabitacionResponseDTO> CreateAsync(CreateHabitacionDto dto)
    {
        var entity = dto.Adapt<Habitacione>();
        _db.Habitaciones.Add(entity);
        await _db.SaveChangesAsync();

        // Insersion de historial de estado inicial
        _db.HistorialEstadoHabitacions.Add(new HistorialEstadoHabitacion
        {
            IdHabitacion = entity.IdHabitacion,
            IdEstadoNuevo = entity.IdEstado,
            FechaCambio = DateTime.Now,
            Observacion = "Creación de habitación"
        });

        await _db.SaveChangesAsync();

        return await GetByIdAsync(entity.IdHabitacion) ?? throw new Exception("Error al recuperar la habitación creada.");
    }

    public async Task<HabitacionResponseDTO?> UpdateAsync(int id, UpdateHabitacionDto dto)
    {
        var entity = await _db.Habitaciones.FindAsync(id);
        if (entity == null) return null;

        // Registro en el historial si se cambia el estado de la habitación
        if (entity.IdEstado != dto.IdEstado)
        {
            _db.HistorialEstadoHabitacions.Add(new HistorialEstadoHabitacion
            {
                IdHabitacion = entity.IdHabitacion,
                IdEstadoAnterior = entity.IdEstado,
                IdEstadoNuevo = dto.IdEstado,
                FechaCambio = DateTime.Now,
                Observacion = "Actualización de estado de habitación"
            });
        }

        dto.Adapt(entity);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(entity.IdHabitacion);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Habitaciones.FindAsync(id);
        if (entity == null) return false;

        _db.Habitaciones.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Exito, string? Mensaje)> CambiarEstadoAsync(int id, CambiarEstadoHabitacionDto dto)
    {
        var entity = await _db.Habitaciones
            .Include(h => h.IdEstadoNavigation)
            .FirstOrDefaultAsync(h => h.IdHabitacion == id);

        if (entity == null) return (false, "Habitación no encontrada.");

        if (entity.IdEstadoNavigation == null) return (false, "La habitación no tiene un estado actual válido.");

        var estadoNuevoEnt = await _db.CatEstadoHabitacions.FindAsync(dto.IdNuevoEstado);
        if (estadoNuevoEnt == null) return (false, "El estado destino no existe.");

        string estadoActualNombre = entity.IdEstadoNavigation.Nombre;
        string estadoNuevoNombre = estadoNuevoEnt.Nombre;

        // Validación con Lua
        var result = _lua.CallFunction("validar_estado_habitacion.lua", "Validar_trancision", estadoActualNombre, estadoNuevoNombre);
        if (result == null || result.Length < 2 || result[0] is not bool valido)
            return (false, "Error al evaluar la transición con Lua.");

        if (!valido)
        {
            string mensajeLua = result.Length > 1 && result[1] is string msg ? msg : "Transición no permitida.";
            return (false, mensajeLua);
        }

        // Proceder al cambio
        var estadoAnterior = entity.IdEstado;
        entity.IdEstado = dto.IdNuevoEstado;
        entity.FechaUltimoCambio = DateTime.Now;
        entity.UsuarioCambio = dto.IdUsuario;

        _db.HistorialEstadoHabitacions.Add(new HistorialEstadoHabitacion
        {
            IdHabitacion = id,
            IdEstadoAnterior = estadoAnterior,
            IdEstadoNuevo = dto.IdNuevoEstado,
            FechaCambio = DateTime.Now,
            IdUsuario = dto.IdUsuario,
            Observacion = dto.Observacion
        });

        await _db.SaveChangesAsync();
        return (true, "Estado cambiado exitosamente.");
    }

    public async Task<List<HabitacionResponseDTO>> GetHabitacionesPorEstadoAsync(string estado)
    {
        return await _db.Habitaciones
            .Include(h => h.IdTipoNavigation)
            .Include(h => h.IdEstadoNavigation)
            .Where(h => h.IdEstadoNavigation!.Nombre == estado)
            .ProjectToType<HabitacionResponseDTO>()
            .ToListAsync();
    }

}