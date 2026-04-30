namespace LaRicaNoche.Api.DTOs.Response;

public sealed record VentaResponseDto
{
    public int IdVenta { get; init; }
    public int? IdCliente { get; init; }
    public string? ClienteNombre { get; init; }
    public DateTime FechaVenta { get; init; }
    public decimal Total { get; init; }
    public string? MetodoPago { get; init; }
    public List<ItemVentaResponseDto> Items { get; init; } = new();
}

public sealed record ItemVentaResponseDto
{
    public int IdItem { get; init; }
    public int IdProducto { get; init; }
    public string? NombreProducto { get; init; }
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal Subtotal { get; init; }
}