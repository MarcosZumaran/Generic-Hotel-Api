namespace HotelGenericoApi.DTOs.Response;

public sealed record HabitacionEstadoActualDto(
    int IdHabitacion,
    string NumeroHabitacion,
    int? Piso,
    string NombreTipo,
    decimal PrecioNoche,
    int IdEstado,
    string NombreEstado,
    string? Descripcion,
    int? IdEstanciaActiva,
    string? ClienteHuesped,
    List<string> AccionesDisponibles,
    DateTime? FechaCheckin,
    DateTime? FechaCheckoutPrevista,
    DateTime? FechaReservaEntrada
);