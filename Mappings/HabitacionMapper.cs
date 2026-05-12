using HotelGenericoApi.Models;
using HotelGenericoApi.DTOs.Request;
using Riok.Mapperly.Abstractions;

namespace HotelGenericoApi.Mappings;

[Mapper]
public partial class HabitacionMapper
{
    [MapperIgnoreTarget(nameof(Habitacion.IdHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacion.FechaUltimoCambio))]
    [MapperIgnoreTarget(nameof(Habitacion.UsuarioCambio))]
    [MapperIgnoreTarget(nameof(Habitacion.Estado))]
    [MapperIgnoreTarget(nameof(Habitacion.TipoHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacion.UsuarioCambioRel))]
    [MapperIgnoreTarget(nameof(Habitacion.Estancias))]
    [MapperIgnoreTarget(nameof(Habitacion.Reservas))]
    public partial Habitacion FromCreate(HabitacionCreateDto dto);

    [MapperIgnoreTarget(nameof(Habitacion.IdHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacion.NumeroHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacion.FechaUltimoCambio))]
    [MapperIgnoreTarget(nameof(Habitacion.UsuarioCambio))]
    [MapperIgnoreTarget(nameof(Habitacion.Estado))]
    [MapperIgnoreTarget(nameof(Habitacion.TipoHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacion.UsuarioCambioRel))]
    [MapperIgnoreTarget(nameof(Habitacion.Estancias))]
    [MapperIgnoreTarget(nameof(Habitacion.Reservas))]
    public partial void UpdateFromDto(HabitacionUpdateDto dto, Habitacion entity);
}