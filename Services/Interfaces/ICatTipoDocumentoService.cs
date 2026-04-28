using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces;

public interface ICatTipoDocumentoService
{
    Task<IEnumerable<CatTipoDocumentoResponseDto>> GetAllAsync();
    Task<CatTipoDocumentoResponseDto?> GetByIdAsync(string codigo);
    Task<CatTipoDocumentoResponseDto> CreateAsync(CatTipoDocumentoCreateDto dto);
    Task<bool> UpdateAsync(string codigo, CatTipoDocumentoUpdateDto dto);
    Task<bool> DeleteAsync(string codigo);
}