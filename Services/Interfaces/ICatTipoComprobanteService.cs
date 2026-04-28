using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces;

public interface ICatTipoComprobanteService
{
    Task<IEnumerable<CatTipoComprobanteResponseDto>> GetAllAsync();
    Task<CatTipoComprobanteResponseDto?> GetByIdAsync(string codigo);
    Task<CatTipoComprobanteResponseDto> CreateAsync(CatTipoComprobanteCreateDto dto);
    Task<bool> UpdateAsync(string codigo, CatTipoComprobanteUpdateDto dto);
    Task<bool> DeleteAsync(string codigo);
}