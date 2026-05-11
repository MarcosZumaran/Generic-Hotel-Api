namespace HotelGenericoApi.DTOs.Response;

public class TopProductoDto
{
    public string Nombre { get; set; } = string.Empty;
    public int CantidadTotal { get; set; }
    public decimal IngresoTotal { get; set; }
}