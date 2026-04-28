using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces;

public interface ICatEstadoHabitacionService
{
    Task<IEnumerable<CatEstadoHabitacionResponseDto>> GetAllAsync();
    Task<CatEstadoHabitacionResponseDto?> GetByIdAsync(int id);
    Task<CatEstadoHabitacionResponseDto> CreateAsync(CatEstadoHabitacionCreateDto dto);
    Task<bool> UpdateAsync(int id, CatEstadoHabitacionUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}