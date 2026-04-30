namespace LaRicaNoche.Api.DTOs.Response;

public sealed record EstadoHabitacionResponseDto(
    string NumeroHabitacion,
    string TipoHabitacion,
    string Estado,
    decimal PrecioNoche,
    DateTime? FechaUltimoCambio
);