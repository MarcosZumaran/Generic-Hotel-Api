namespace LaRicaNoche.Api.DTOs.Request;

public sealed record VentaCreateDto
{
    public int? IdCliente { get; init; }       // null si es venta anónima
    public string MetodoPago { get; init; } = "005";
    public List<ItemVentaCreateDto> Items { get; init; } = new();
}

public sealed record ItemVentaCreateDto
{
    public int IdProducto { get; init; }
    public int Cantidad { get; init; }
}