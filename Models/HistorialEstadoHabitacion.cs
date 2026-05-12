using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class HistorialEstadoHabitacion
{
    public int IdHistorial { get; set; }

    public int IdHabitacion { get; set; }

    public int? IdEstadoAnterior { get; set; }

    public int? IdEstadoNuevo { get; set; }

    public DateTime? FechaCambio { get; set; }

    public int? IdUsuario { get; set; }

    public string? Observacion { get; set; }

    public virtual Habitacion Habitacion { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}