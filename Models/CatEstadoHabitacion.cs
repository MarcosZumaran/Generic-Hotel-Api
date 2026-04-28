using System;
using System.Collections.Generic;

namespace LaRicaNoche.Api.Models;

public partial class CatEstadoHabitacion
{
    public int IdEstado { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Habitacione> Habitaciones { get; set; } = new List<Habitacione>();
}
