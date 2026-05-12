using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class VCierreCajaDiario
{
    public DateOnly? Fecha { get; set; }

    public string MetodoPago { get; set; } = null!;

    public decimal? Ingresos { get; set; }

    public string Concepto { get; set; } = null!;
}