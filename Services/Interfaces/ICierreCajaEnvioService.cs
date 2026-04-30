using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces
{
    public interface ICierreCajaEnvioService
    {
        Task<CierreCajaEnvioDto> GetEstadoAsync(DateOnly fecha);
        Task<bool> MarcarComoEnviadoAsync(DateOnly fecha);
    }
}