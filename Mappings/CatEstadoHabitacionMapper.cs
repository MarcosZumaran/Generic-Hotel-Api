using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class EstadoHabitacionMapper
{
    public partial CatEstadoHabitacionResponseDto ToResponse(EstadoHabitacion entity);
    public partial EstadoHabitacion FromCreate(CatEstadoHabitacionCreateDto dto);
    public partial void UpdateFromDto(CatEstadoHabitacionUpdateDto dto, EstadoHabitacion entity);
}