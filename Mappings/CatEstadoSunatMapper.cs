using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class EstadoSunatMapper
{
    public partial CatEstadoSunatResponseDto ToResponse(EstadoSunat entity);
    public partial EstadoSunat FromCreate(CatEstadoSunatCreateDto dto);
    public partial void UpdateFromDto(CatEstadoSunatUpdateDto dto, EstadoSunat entity);
}