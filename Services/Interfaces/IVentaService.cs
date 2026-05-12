using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;

namespace HotelGenericoApi.Services.Interfaces;

public interface IVentaService
{
    Task<IEnumerable<VentaResponseDto>> GetAllAsync();
    Task<VentaResponseDto?> GetByIdAsync(int id);
    Task<VentaResponseDto> CreateAsync(VentaCreateDto dto, int? idUsuario);
    Task<PagedResult<VentaResponseDto>> GetPagedAsync(int page, int pageSize);
}
