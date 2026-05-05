using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Models;

namespace HotelGenericoApi.Services.Implementations
{
    public class HabitacionService : IHabitacionService
    {
        private readonly HotelDbContext _db;
        private readonly HabitacionMapper _mapper;

        public HabitacionService(HotelDbContext db, HabitacionMapper mapper)
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

            int? estadoAnterior = entity.IdEstado;

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
                _db.HistorialEstadoHabitacions.Add(historial);
            }

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

        public async Task<IEnumerable<HabitacionEstadoActualDto>> GetEstadoActualAsync(string? rolUsuario)
        {
            var habitaciones = await _db.Habitaciones
                .Include(h => h.IdTipoNavigation)
                .Include(h => h.IdEstadoNavigation)
                .Include(h => h.Estancia.Where(e => e.Estado == "Activa"))
                    .ThenInclude(e => e.IdClienteTitularNavigation)
                .AsNoTracking()
                .ToListAsync();

            return habitaciones.Select(h => new HabitacionEstadoActualDto(
                h.IdHabitacion,
                h.NumeroHabitacion,
                h.Piso,
                h.IdTipoNavigation?.Nombre ?? "",
                h.PrecioNoche,
                h.IdEstado ?? 0,
                h.IdEstadoNavigation?.Nombre ?? "",
                h.Descripcion,
                h.Estancia.FirstOrDefault()?.IdEstancia,
                h.Estancia.FirstOrDefault()?.IdClienteTitularNavigation != null
                    ? $"{h.Estancia.First().IdClienteTitularNavigation!.Nombres} {h.Estancia.First().IdClienteTitularNavigation!.Apellidos}"
                    : null,
                ObtenerAccionesDisponibles(h.IdEstadoNavigation, rolUsuario)
            )).ToList();
        }

        private static List<string> ObtenerAccionesDisponibles(CatEstadoHabitacion? estado, string? rolUsuario)
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
}