using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces;

public interface IReporteService
{
    Task<IEnumerable<CierreCajaResponseDto>> GetCierreCajaAsync(DateOnly? fecha);
    Task<IEnumerable<EstadoHabitacionResponseDto>> GetEstadoHabitacionesAsync();
}