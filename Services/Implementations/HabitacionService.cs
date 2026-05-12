using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Models;
using HotelGenericoApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HotelGenericoApi.Services.Implementations
{
    public class HabitacionService : IHabitacionService
    {
        private readonly HotelDbContext _db;
        private readonly HabitacionMapper _mapper;
        private readonly IValidadorEstadoService _validadorEstadoService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<HabitacionHub> _hubContext;

        public HabitacionService(HotelDbContext db, HabitacionMapper mapper, IValidadorEstadoService validador, Microsoft.AspNetCore.SignalR.IHubContext<HabitacionHub> hubContext)
        {
            _db = db;
            _mapper = mapper;
            _validadorEstadoService = validador;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<HabitacionResponseDto>> GetAllAsync()
        {
            var entities = await _db.Habitaciones
                .Include(h => h.TipoHabitacion)
                .Include(h => h.Estado)
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(MapToResponse);
        }

        public async Task<HabitacionResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Habitaciones
                .Include(h => h.TipoHabitacion)
                .Include(h => h.Estado)
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

            await _hubContext.Clients.All.SendAsync("EstadoHabitacionCambiado", new
            {
                entity.IdHabitacion,
                entity.NumeroHabitacion,
                entity.IdEstado,
                entity.FechaUltimoCambio
            });

            await _db.Entry(entity).Reference(h => h.TipoHabitacion).LoadAsync();
            await _db.Entry(entity).Reference(h => h.Estado).LoadAsync();

            return MapToResponse(entity);
        }

        public async Task<bool> UpdateAsync(int id, HabitacionUpdateDto dto, int? idUsuario)
        {
            var entity = await _db.Habitaciones.FindAsync(id);
            if (entity is null) return false;

            int? estadoAnterior = entity.IdEstado;

            // Validar transición si se está cambiando el estado
            if (dto.IdEstado.HasValue)
            {
                bool permitida = await _validadorEstadoService.EsTransicionValidaAsync(estadoAnterior ?? 0, dto.IdEstado.Value);
                if (!permitida)
                    throw new InvalidOperationException("Transición de estado no permitida.");
            }

            if (dto.Piso.HasValue) entity.Piso = dto.Piso.Value;
            if (dto.Descripcion != null) entity.Descripcion = dto.Descripcion;
            if (dto.IdTipo.HasValue) entity.IdTipo = dto.IdTipo.Value;
            if (dto.PrecioNoche.HasValue) entity.PrecioNoche = dto.PrecioNoche.Value;
            if (dto.IdEstado.HasValue) entity.IdEstado = dto.IdEstado.Value;

            if (dto.IdEstado.HasValue && estadoAnterior != dto.IdEstado.Value)
            {
                var historial = new HistorialEstadoHabitacion
                {
                    IdHabitacion = entity.IdHabitacion,
                    IdEstadoAnterior = estadoAnterior,
                    IdEstadoNuevo = dto.IdEstado.Value,
                    FechaCambio = DateTime.UtcNow,
                    IdUsuario = idUsuario,
                    Observacion = "Cambio manual de estado"
                };
                _db.HistorialesEstadoHabitacion.Add(historial);
            }

            entity.FechaUltimoCambio = DateTime.UtcNow;
            entity.UsuarioCambio = idUsuario;

            await _db.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("EstadoHabitacionCambiado", new
            {
                IdHabitacion = id,
                entity.NumeroHabitacion,
                IdEstado = entity.IdEstado,
                entity.FechaUltimoCambio
            });

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

            await _hubContext.Clients.All.SendAsync("EstadoHabitacionCambiado", new
            {
                IdHabitacion = id,
                NumeroHabitacion = "",
                IdEstado = -1, // Esto indica que la habitación fue eliminada
                FechaUltimoCambio = DateTime.UtcNow
            });

            return true;
        }

        public async Task<IEnumerable<HabitacionEstadoActualDto>> GetEstadoActualAsync(string? rolUsuario)
        {
            var habitaciones = await _db.Habitaciones
                .Include(h => h.TipoHabitacion)
                .Include(h => h.Estado)
                .Include(h => h.Estancias.Where(e => e.Estado == "Activa"))
                    .ThenInclude(e => e.ClienteTitular)
                .AsNoTracking()
                .ToListAsync();

            return habitaciones.Select(h => new HabitacionEstadoActualDto(
                h.IdHabitacion,
                h.NumeroHabitacion,
                h.Piso,
                h.TipoHabitacion?.Nombre ?? "",
                h.PrecioNoche,
                h.IdEstado,
                h.Estado?.Nombre ?? "",
                h.Descripcion,
                h.Estancias.FirstOrDefault()?.IdEstancia,
                h.Estancias.FirstOrDefault()?.ClienteTitular != null
                    ? $"{h.Estancias.First().ClienteTitular!.Nombres} {h.Estancias.First().ClienteTitular!.Apellidos}"
                    : null,
                ObtenerAccionesDisponibles(h.Estado, rolUsuario)
            )).ToList();
        }

        private static List<string> ObtenerAccionesDisponibles(EstadoHabitacion? estado, string? rolUsuario)
        {
            var acciones = new List<string>();

            if (estado == null) return acciones;

            if (estado.PermiteCheckin && (rolUsuario == "Administrador" || rolUsuario == "Recepcionista"))
                acciones.Add("CheckIn");

            if (estado.PermiteCheckout && (rolUsuario == "Administrador" || rolUsuario == "Recepcionista"))
            {
                acciones.Add("CheckOut");
                acciones.Add("PasarLimpieza");
            }

            if (rolUsuario == "Administrador")
            {
                if (estado.Nombre == "Disponible") acciones.Add("Mantenimiento");
                if (estado.Nombre == "Limpieza") acciones.Add("FinalizarLimpieza");
                if (estado.Nombre == "Mantenimiento") acciones.Add("Habilitar");
            }

            if (rolUsuario == "Limpieza" && estado.Nombre == "Limpieza")
                acciones.Add("FinalizarLimpieza");

            return acciones;
        }

        private static HabitacionResponseDto MapToResponse(Habitacion h)
        {
            return new HabitacionResponseDto(
                h.IdHabitacion,
                h.NumeroHabitacion,
                h.Piso,
                h.Descripcion,
                h.IdTipo,
                h.TipoHabitacion?.Nombre ?? "",
                h.PrecioNoche,
                h.IdEstado,
                h.Estado?.Nombre ?? "",
                h.FechaUltimoCambio,
                h.UsuarioCambio
            );
        }
    }
}