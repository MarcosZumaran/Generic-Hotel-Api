using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class MetodoPago
{
    public string Codigo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;


    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}