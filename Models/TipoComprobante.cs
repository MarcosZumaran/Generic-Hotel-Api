using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class TipoComprobante
{
    public string Codigo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();
}