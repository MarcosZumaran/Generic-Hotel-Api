using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class TipoDocumento
{
    public string Codigo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

}