namespace LaRicaNoche.Api.DTOs.Request;

public sealed record CatEstadoHabitacionUpdateDto
{
    public string? Nombre { get; init; }
    public string? Descripcion { get; init; }
}