namespace LaRicaNoche.Api.DTOs.Response;

public sealed record HabitacionResponseDto(
    int IdHabitacion,
    string NumeroHabitacion,
    int? Piso,
    string? Descripcion,
    int IdTipo,
    string NombreTipo,
    decimal PrecioNoche,
    int? IdEstado,
    string? NombreEstado,
    DateTime? FechaUltimoCambio,
    int? UsuarioCambio
);