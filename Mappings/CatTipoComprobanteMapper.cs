using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class TipoComprobanteMapper
{
    public partial CatTipoComprobanteResponseDto ToResponse(TipoComprobante entity);
    public partial TipoComprobante FromCreate(CatTipoComprobanteCreateDto dto);
    public partial void UpdateFromDto(CatTipoComprobanteUpdateDto dto, TipoComprobante entity);
}