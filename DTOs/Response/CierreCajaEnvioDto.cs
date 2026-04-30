namespace LaRicaNoche.Api.DTOs.Response
{
    public sealed record CierreCajaEnvioDto(
        DateOnly Fecha,
        int? IdEstadoSunat,
        string? NombreEstadoSunat,
        DateTime? FechaEnvio,
        int? IntentosEnvio
    );
}