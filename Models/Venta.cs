using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public int? IdCliente { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? FechaVenta { get; set; }

    public decimal Total { get; set; }

    public string MetodoPago { get; set; } = null!;

    public virtual ICollection<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();

    public virtual Cliente? Cliente { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<ItemVenta> ItemsVenta { get; set; } = new List<ItemVenta>();

    public virtual MetodoPago MetodoPagoRel { get; set; } = null!;
}
