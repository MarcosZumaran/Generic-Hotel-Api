using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces;

public interface IVentaService
{
    Task<IEnumerable<VentaResponseDto>> GetAllAsync();
    Task<VentaResponseDto?> GetByIdAsync(int id);
    Task<VentaResponseDto> CreateAsync(VentaCreateDto dto, int? idUsuario);
}