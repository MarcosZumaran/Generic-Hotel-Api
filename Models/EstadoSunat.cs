using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class EstadoSunat
{
    public int Codigo { get; set; }

    public string Descripcion { get; set; } = null!;

    public string? DescripcionLarga { get; set; }

    public virtual ICollection<CierreCajaEnvio> CierreCajaEnvios { get; set; } = new List<CierreCajaEnvio>();

    public virtual ICollection<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();
}