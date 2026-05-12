using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Temporada
{
    public int IdTemporada { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal Multiplicador { get; set; }

    public virtual ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
}
