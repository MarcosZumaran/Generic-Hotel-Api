using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Tarifa
{
    public int IdTarifa { get; set; }

    public int IdTipoHabitacion { get; set; }

    public int? IdTemporada { get; set; }

    public decimal Precio { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public virtual TipoHabitacion TipoHabitacion { get; set; } = null!;

    public virtual Temporada? Temporada { get; set; }
}