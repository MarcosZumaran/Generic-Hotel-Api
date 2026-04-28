namespace LaRicaNoche.Api.DTOs.Request;

public sealed record CatEstadoHabitacionCreateDto
{
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
}