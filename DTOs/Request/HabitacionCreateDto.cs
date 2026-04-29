namespace LaRicaNoche.Api.DTOs.Request;

public sealed record HabitacionCreateDto
{
    public string NumeroHabitacion { get; init; } = string.Empty;
    public int? Piso { get; init; }
    public string? Descripcion { get; init; }
    public int IdTipo { get; init; }
    public decimal PrecioNoche { get; init; }
    public int? IdEstado { get; init; } // normalmente omitido (usa default 1)
}