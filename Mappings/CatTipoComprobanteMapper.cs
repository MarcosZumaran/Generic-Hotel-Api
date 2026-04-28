using LaRicaNoche.Api.Models;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace LaRicaNoche.Api.Mappings;

[Mapper]
public partial class CatTipoComprobanteMapper
{
    public partial CatTipoComprobanteResponseDto ToResponse(CatTipoComprobante entity);
    public partial CatTipoComprobante FromCreate(CatTipoComprobanteCreateDto dto);
    public partial void UpdateFromDto(CatTipoComprobanteUpdateDto dto, CatTipoComprobante entity);
}