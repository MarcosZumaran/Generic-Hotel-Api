namespace LaRicaNoche.Api.DTOs.Request;

public sealed record HabitacionUpdateDto
{
    public int? Piso { get; init; }
    public string? Descripcion { get; init; }
    public int? IdTipo { get; init; }
    public decimal? PrecioNoche { get; init; }
    public int? IdEstado { get; init; }
}