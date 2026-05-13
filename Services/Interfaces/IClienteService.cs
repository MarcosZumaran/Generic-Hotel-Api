using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;

namespace HotelGenericoApi.Services.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteResponseDto>> GetAllAsync();
    Task<ClienteResponseDto?> GetByIdAsync(int id);
    Task<ClienteResponseDto?> GetByDocumentoAsync(string tipoDocumento, string documento);
    Task<ClienteResponseDto> CreateAsync(ClienteCreateDto dto);
    Task<bool> UpdateAsync(int id, ClienteUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<PagedResult<ClienteResponseDto>> GetPagedAsync(int page, int pageSize);
    Task<IEnumerable<ClienteResponseDto>> BuscarAsync(string termino, int maxResults);
}