using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string TipoDocumento { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Nacionalidad { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Direccion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaVerificacionReniec { get; set; }

    public virtual TipoDocumento TipoDocumentoRel { get; set; } = null!;

    public virtual ICollection<Estancium> Estancias { get; set; } = new List<Estancium>();

    public virtual ICollection<Huesped> Huespedes { get; set; } = new List<Huesped>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}