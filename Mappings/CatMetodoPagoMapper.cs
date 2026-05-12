using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class MetodoPagoMapper
{
    public partial CatMetodoPagoResponseDto ToResponse(MetodoPago entity);
    public partial MetodoPago FromCreate(CatMetodoPagoCreateDto dto);
    public partial void UpdateFromDto(CatMetodoPagoUpdateDto dto, MetodoPago entity);
}