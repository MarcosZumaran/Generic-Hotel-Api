using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class ClienteMapper
{
    public partial ClienteResponseDto ToResponse(Cliente entity);

    [MapperIgnoreTarget(nameof(Cliente.IdCliente))]
    [MapperIgnoreTarget(nameof(Cliente.FechaRegistro))]
    [MapperIgnoreTarget(nameof(Cliente.FechaVerificacionReniec))]
    [MapperIgnoreTarget(nameof(Cliente.TipoDocumentoRel))]
    [MapperIgnoreTarget(nameof(Cliente.Estancias))]
    [MapperIgnoreTarget(nameof(Cliente.Huespedes))]
    [MapperIgnoreTarget(nameof(Cliente.Reservas))]
    [MapperIgnoreTarget(nameof(Cliente.Ventas))]
    public partial Cliente FromCreate(ClienteCreateDto dto);

    [MapperIgnoreTarget(nameof(Cliente.IdCliente))]
    [MapperIgnoreTarget(nameof(Cliente.TipoDocumento))]
    [MapperIgnoreTarget(nameof(Cliente.Documento))]
    [MapperIgnoreTarget(nameof(Cliente.FechaRegistro))]
    [MapperIgnoreTarget(nameof(Cliente.FechaVerificacionReniec))]
    [MapperIgnoreTarget(nameof(Cliente.TipoDocumentoRel))]
    [MapperIgnoreTarget(nameof(Cliente.Estancias))]
    [MapperIgnoreTarget(nameof(Cliente.Huespedes))]
    [MapperIgnoreTarget(nameof(Cliente.Reservas))]
    [MapperIgnoreTarget(nameof(Cliente.Ventas))]
    public partial void UpdateFromDto(ClienteUpdateDto dto, Cliente entity);
}