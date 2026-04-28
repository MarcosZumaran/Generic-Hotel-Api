using LaRicaNoche.Api.Models;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace LaRicaNoche.Api.Mappings;

[Mapper]
public partial class CatEstadoHabitacionMapper
{
    public partial CatEstadoHabitacionResponseDto ToResponse(CatEstadoHabitacion entity);
    public partial CatEstadoHabitacion FromCreate(CatEstadoHabitacionCreateDto dto);
    public partial void UpdateFromDto(CatEstadoHabitacionUpdateDto dto, CatEstadoHabitacion entity);
}