using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class RolUsuarioMapper
{
    public partial CatRolUsuarioResponseDto ToResponse(RolUsuario entity);
    public partial RolUsuario FromCreate(CatRolUsuarioCreateDto dto);
    public partial void UpdateFromDto(CatRolUsuarioUpdateDto dto, RolUsuario entity);
}