using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class TransicionEstado
{
    public int IdTransicion { get; set; }

    public int IdEstadoActual { get; set; }

    public int IdEstadoSiguiente { get; set; }

    public virtual EstadoHabitacion EstadoActual { get; set; } = null!;

    public virtual EstadoHabitacion EstadoSiguiente { get; set; } = null!;
}