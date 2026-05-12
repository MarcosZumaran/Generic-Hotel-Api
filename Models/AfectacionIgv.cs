using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class AfectacionIgv
{
    public string Codigo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}