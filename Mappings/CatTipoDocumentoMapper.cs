using LaRicaNoche.Api.Models;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace LaRicaNoche.Api.Mappings;

[Mapper]
public partial class CatTipoDocumentoMapper
{
    public partial CatTipoDocumentoResponseDto ToResponse(CatTipoDocumento entity);
    public partial CatTipoDocumento FromCreate(CatTipoDocumentoCreateDto dto);
    public partial void UpdateFromDto(CatTipoDocumentoUpdateDto dto, CatTipoDocumento entity);
}