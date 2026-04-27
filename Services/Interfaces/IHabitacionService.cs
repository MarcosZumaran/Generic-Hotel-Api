using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Request.Create;
using LaRicaNoche.Api.DTOs.Request.Update;
using LaRicaNoche.Api.DTOs.Response;

namespace LaRicaNoche.Api.Services.Interfaces;

public interface IHabitacionService
{
    Task<List<HabitacionResponseDTO>> GetAllAsync();
    Task<HabitacionResponseDTO?> GetByIdAsync(int id);
    Task<HabitacionResponseDTO> CreateAsync(CreateHabitacionDto dto);
    Task<HabitacionResponseDTO?> UpdateAsync(int id, UpdateHabitacionDto dto);
    Task<bool> DeleteAsync(int id);
    Task<(bool Exito, string? Mensaje)> CambiarEstadoAsync(int id, CambiarEstadoHabitacionDto dto);
    Task<List<HabitacionResponseDTO>> GetHabitacionesPorEstadoAsync(string estado);
}