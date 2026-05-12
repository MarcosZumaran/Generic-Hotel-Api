using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Habitacion
{
    public int IdHabitacion { get; set; }

    public string NumeroHabitacion { get; set; } = null!;

    public int? Piso { get; set; }

    public string? Descripcion { get; set; }

    public int IdTipo { get; set; }

    public decimal PrecioNoche { get; set; }

    public int IdEstado { get; set; }

    public DateTime? FechaUltimoCambio { get; set; }

    public int? UsuarioCambio { get; set; }

    public virtual EstadoHabitacion Estado { get; set; } = null!;

    public virtual TipoHabitacion TipoHabitacion { get; set; } = null!;

    public virtual Usuario? UsuarioCambioRel { get; set; }

    public virtual ICollection<Estancium> Estancias { get; set; } = new List<Estancium>();

    public virtual ICollection<HistorialEstadoHabitacion> HistorialesEstadoHabitacion { get; set; } = new List<HistorialEstadoHabitacion>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}