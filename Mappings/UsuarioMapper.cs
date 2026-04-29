using LaRicaNoche.Api.Models;
using LaRicaNoche.Api.DTOs.Request;
using Riok.Mapperly.Abstractions;

namespace LaRicaNoche.Api.Mappings;

[Mapper]
public partial class UsuarioMapper
{
    // Ignorar password_hash porque se maneja con bcrypt en el servicio
    [MapperIgnoreTarget(nameof(Usuario.PasswordHash))]
    public partial Usuario FromCreate(UsuarioCreateDto dto);

    // Ignorar propiedades que no queremos mapear desde el DTO de actualización
    [MapperIgnoreTarget(nameof(Usuario.PasswordHash))]
    [MapperIgnoreTarget(nameof(Usuario.FechaCreacion))]
    public partial void UpdateFromDto(UsuarioUpdateDto dto, Usuario entity);
}