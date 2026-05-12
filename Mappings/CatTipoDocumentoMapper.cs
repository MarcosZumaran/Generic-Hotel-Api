using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class TipoDocumentoMapper
{
    public partial CatTipoDocumentoResponseDto ToResponse(TipoDocumento entity);
    public partial TipoDocumento FromCreate(CatTipoDocumentoCreateDto dto);
    public partial void UpdateFromDto(CatTipoDocumentoUpdateDto dto, TipoDocumento entity);
}