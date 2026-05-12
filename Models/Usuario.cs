using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int IdRol { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? EstaActivo { get; set; }

    public virtual RolUsuario Rol { get; set; } = null!;

    public virtual ICollection<Habitacion> HabitacionesModificadas { get; set; } = new List<Habitacion>();

    public virtual ICollection<HistorialEstadoHabitacion> HistorialesEstadoHabitacion { get; set; } = new List<HistorialEstadoHabitacion>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}