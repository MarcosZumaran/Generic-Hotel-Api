using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Huesped
{
    public int IdHuesped { get; set; }

    public int IdEstancia { get; set; }

    public int IdCliente { get; set; }

    public bool? EsTitular { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Estancium Estancia { get; set; } = null!;
}