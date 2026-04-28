namespace LaRicaNoche.Api.DTOs.Response;

public sealed record CatEstadoHabitacionResponseDto(
    int IdEstado,
    string Nombre,
    string? Descripcion
);