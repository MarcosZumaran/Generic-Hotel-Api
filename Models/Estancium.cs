using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Estancium
{
    public int IdEstancia { get; set; }

    public int? IdReserva { get; set; }

    public int IdHabitacion { get; set; }

    public int IdClienteTitular { get; set; }

    public DateTime FechaCheckin { get; set; }

    public DateTime FechaCheckoutPrevista { get; set; }

    public DateTime? FechaCheckoutReal { get; set; }

    public decimal MontoTotal { get; set; }

    public string? Estado { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Habitacion Habitacion { get; set; } = null!;

    public virtual Cliente ClienteTitular { get; set; } = null!;

    public virtual Reserva? Reserva { get; set; }


    public virtual ICollection<Huesped> Huespedes { get; set; } = new List<Huesped>();

    public virtual ICollection<ItemEstancium> ItemsEstancia { get; set; } = new List<ItemEstancium>();
}