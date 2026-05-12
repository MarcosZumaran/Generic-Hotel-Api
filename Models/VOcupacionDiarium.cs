using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class VOcupacionDiarium
{
    public DateOnly? Fecha { get; set; }

    public int? Ocupadas { get; set; }

    public int? Total { get; set; }

    public decimal? PorcentajeOcupacion { get; set; }
}