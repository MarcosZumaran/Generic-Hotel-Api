namespace LaRicaNoche.Api.DTOs.Response;

public sealed record CierreCajaResponseDto(
    DateOnly? Fecha,
    string MetodoPago,
    decimal? Ingresos,
    string Concepto
);