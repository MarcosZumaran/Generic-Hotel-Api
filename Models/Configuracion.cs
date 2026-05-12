using System;
using System.Collections.Generic;

namespace HotelGenericoApi.Models;

public partial class Configuracion
{
    public int IdConfiguracion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Ruc { get; set; }

    public decimal TasaIgvHotel { get; set; }

    public decimal TasaIgvProductos { get; set; }
}