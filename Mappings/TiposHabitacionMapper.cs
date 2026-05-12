using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class TiposHabitacionMapper
{
    public partial TiposHabitacionResponseDto ToResponse(TipoHabitacion entity);
    public partial TipoHabitacion FromCreate(TiposHabitacionCreateDto dto);
    public partial void UpdateFromDto(TiposHabitacionUpdateDto dto, TipoHabitacion entity);
}