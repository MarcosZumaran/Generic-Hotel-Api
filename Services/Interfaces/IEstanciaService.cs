using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;

namespace HotelGenericoApi.Services.Interfaces;

public interface IEstanciaService
{
    Task<IEnumerable<EstanciaResponseDto>> GetAllAsync();
    Task<EstanciaResponseDto?> GetByIdAsync(int id);
    Task<EstanciaResponseDto> CheckInAsync(CheckInDto dto, int? idUsuario);
    Task<EstanciaResponseDto> CheckOutAsync(int idEstancia, int? idUsuario);
    Task<EstanciaResponseDto> RegistrarConsumoAsync(int idEstancia, ConsumoEstanciaCreateDto dto, int? idUsuario);
    Task<ReservaResponseDto> CrearReservaAsync(ReservaCreateDto dto, int? idUsuario);
    Task<ReservaResponseDto?> GetReservaByIdAsync(int id);
    Task<IEnumerable<ReservaResponseDto>> GetReservasPorHabitacionAsync(int idHabitacion);
    Task<IEnumerable<ItemConsumoResponseDto>> GetConsumosAsync(int idEstancia);
    Task ActualizarConsumoAsync(int idEstancia, int idItem, int nuevaCantidad, int? idUsuario);
    Task EliminarConsumoAsync(int idEstancia, int idItem, int? idUsuario);
    Task CancelarReservaAsync(int idReserva, int? idUsuario);
}