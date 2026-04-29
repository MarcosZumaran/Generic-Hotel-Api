using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces;

public interface IHabitacionService
{
    Task<IEnumerable<HabitacionResponseDto>> GetAllAsync();
    Task<HabitacionResponseDto?> GetByIdAsync(int id);
    Task<HabitacionResponseDto> CreateAsync(HabitacionCreateDto dto, int? idUsuario);
    Task<bool> UpdateAsync(int id, HabitacionUpdateDto dto, int? idUsuario);
    Task<bool> DeleteAsync(int id);
}