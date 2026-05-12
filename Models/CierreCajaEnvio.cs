using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class CierreCajaEnvio
{
    public DateOnly Fecha { get; set; }

    public int IdEstadoSunat { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public int? IntentosEnvio { get; set; }

    public string? HashXml { get; set; }

    public virtual EstadoSunat EstadoSunat { get; set; } = null!;
}