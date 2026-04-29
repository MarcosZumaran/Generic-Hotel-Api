using LaRicaNoche.Api.Models;
using LaRicaNoche.Api.DTOs.Request;
using Riok.Mapperly.Abstractions;

namespace LaRicaNoche.Api.Mappings;

[Mapper]
public partial class HabitacionMapper
{

    [MapperIgnoreTarget(nameof(Habitacione.IdHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacione.FechaUltimoCambio))]
    [MapperIgnoreTarget(nameof(Habitacione.UsuarioCambio))]
    [MapperIgnoreTarget(nameof(Habitacione.IdEstadoNavigation))]
    [MapperIgnoreTarget(nameof(Habitacione.IdTipoNavigation))]
    [MapperIgnoreTarget(nameof(Habitacione.UsuarioCambioNavigation))]
    [MapperIgnoreTarget(nameof(Habitacione.Estancia))]
    [MapperIgnoreTarget(nameof(Habitacione.HistorialEstadoHabitacions))]
    [MapperIgnoreTarget(nameof(Habitacione.Reservas))]
    public partial Habitacione FromCreate(HabitacionCreateDto dto);

    [MapperIgnoreTarget(nameof(Habitacione.IdHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacione.NumeroHabitacion))]
    [MapperIgnoreTarget(nameof(Habitacione.FechaUltimoCambio))]
    [MapperIgnoreTarget(nameof(Habitacione.UsuarioCambio))]
    [MapperIgnoreTarget(nameof(Habitacione.IdEstadoNavigation))]
    [MapperIgnoreTarget(nameof(Habitacione.IdTipoNavigation))]
    [MapperIgnoreTarget(nameof(Habitacione.UsuarioCambioNavigation))]
    [MapperIgnoreTarget(nameof(Habitacione.Estancia))]
    [MapperIgnoreTarget(nameof(Habitacione.HistorialEstadoHabitacions))]
    [MapperIgnoreTarget(nameof(Habitacione.Reservas))]
    public partial void UpdateFromDto(HabitacionUpdateDto dto, Habitacione entity);
}