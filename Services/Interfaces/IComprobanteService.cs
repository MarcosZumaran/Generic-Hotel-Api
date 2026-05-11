using HotelGenericoApi.DTOs.Response;

namespace HotelGenericoApi.Services.Interfaces;

public interface IComprobanteService
{
    Task<IEnumerable<ComprobanteResponseDto>> GetAllAsync();
    Task<ComprobanteResponseDto?> GetByIdAsync(int id);
    Task<bool> MarcarComoEnviadoAsync(int id, string hashXml);
    Task<PagedResult<ComprobanteResponseDto>> GetPagedAsync(int page, int pageSize);
}