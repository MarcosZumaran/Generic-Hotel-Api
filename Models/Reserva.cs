using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int IdCliente { get; set; }

    public int IdHabitacion { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime FechaEntradaPrevista { get; set; }

    public DateTime FechaSalidaPrevista { get; set; }

    public decimal MontoTotal { get; set; }

    public string? Estado { get; set; }

    public string? Observaciones { get; set; }
    public bool EsNoShow { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Habitacion Habitacion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<Estancium> Estancias { get; set; } = new List<Estancium>();
}