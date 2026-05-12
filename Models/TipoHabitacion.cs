using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class TipoHabitacion
{
    public int IdTipo { get; set; }

    public string Nombre { get; set; } = null!;

    public int? Capacidad { get; set; }

    public string? Descripcion { get; set; }

    public decimal? PrecioBase { get; set; }

    public virtual ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();

    public virtual ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
}