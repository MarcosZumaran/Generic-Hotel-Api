using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class RolUsuario
{
    public int IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}