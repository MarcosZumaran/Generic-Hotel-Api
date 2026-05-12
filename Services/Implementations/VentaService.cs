using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Models;
using HotelGenericoApi.Models.Exceptions;
using HotelGenericoApi.Services.Interfaces;
using NLua;
using HotelGenericoApi.Extensions;

namespace HotelGenericoApi.Services.Implementations;

public class VentaService : IVentaService
{
    private readonly HotelDbContext _db;
    private readonly ILuaService _lua;

    public VentaService(HotelDbContext db, ILuaService lua)
    {
        _db = db;
        _lua = lua;
    }

    public async Task<IEnumerable<VentaResponseDto>> GetAllAsync()
    {
        return await _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.ItemsVenta).ThenInclude(i => i.Producto)
            .AsNoTracking()
            .OrderByDescending(v => v.FechaVenta)
            .Select(v => new VentaResponseDto
            {
                IdVenta = v.IdVenta,
                IdCliente = v.IdCliente,
                ClienteNombre = v.Cliente != null
                    ? $"{v.Cliente.Nombres} {v.Cliente.Apellidos}"
                    : "CLIENTE ANÓNIMO",
                FechaVenta = v.FechaVenta ?? DateTime.MinValue,
                Total = v.Total,
                MetodoPago = v.MetodoPago,
                Items = v.ItemsVenta.Select(i => new ItemVentaResponseDto
                {
                    IdItem = i.IdItem,
                    IdProducto = i.IdProducto,
                    NombreProducto = i.Producto != null ? i.Producto.Nombre : "",
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario,
                    Subtotal = i.Subtotal ?? 0
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<VentaResponseDto?> GetByIdAsync(int id)
    {
        var v = await _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.ItemsVenta).ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(x => x.IdVenta == id);

        if (v == null) return null;

        return new VentaResponseDto
        {
            IdVenta = v.IdVenta,
            IdCliente = v.IdCliente,
            ClienteNombre = v.Cliente != null
                ? $"{v.Cliente.Nombres} {v.Cliente.Apellidos}"
                : "CLIENTE ANÓNIMO",
            FechaVenta = v.FechaVenta ?? DateTime.MinValue,
            Total = v.Total,
            MetodoPago = v.MetodoPago,
            Items = v.ItemsVenta.Select(i => new ItemVentaResponseDto
            {
                IdItem = i.IdItem,
                IdProducto = i.IdProducto,
                NombreProducto = i.Producto?.Nombre ?? "",
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                Subtotal = i.Subtotal ?? 0
            }).ToList()
        };
    }

    public async Task<VentaResponseDto> CreateAsync(VentaCreateDto dto, int? idUsuario)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            throw new BusinessRuleViolationException(BusinessErrorCode.ValidationError, "La venta debe tener al menos un producto.");

        // 1. Obtener cliente (puede ser null si es anónimo)
        Cliente? cliente = null;
        if (dto.IdCliente.HasValue)
        {
            cliente = await _db.Clientes.FindAsync(dto.IdCliente.Value);
            if (cliente == null)
                throw new BusinessRuleViolationException(BusinessErrorCode.ClientNotFound, "El cliente no existe.");
        }
        else
        {
            cliente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.TipoDocumento == "0" && c.Documento == "00000000");
            if (cliente == null)
                throw new BusinessRuleViolationException(BusinessErrorCode.ClientNotFound, "Cliente anónimo no configurado.");
        }

// 2. Calcular totales y crear ItemVenta

        var itemsVenta = new List<ItemVenta>();
        decimal montoSinIgvTotal = 0;
        decimal igvTotal = 0;

        foreach (var item in dto.Items)
        {
            var producto = await _db.Productos.FindAsync(item.IdProducto);
            if (producto == null)
                throw new BusinessRuleViolationException(BusinessErrorCode.ProductNotFound, $"El producto con ID {item.IdProducto} no existe.");

            decimal montoSinIgvItem = producto.PrecioUnitario * item.Cantidad;
            montoSinIgvTotal += montoSinIgvItem;

            // Cálculo de IGV con Lua (usamos el script de reglas generales, no el hotelero)
            decimal tasaItem = 18m; // valor por defecto para código '10'
            decimal igvItem = montoSinIgvItem * (tasaItem / 100);
            try
            {
                var luaArgs = new object[] { producto.IdAfectacionIgv ?? "10", montoSinIgvItem, "03" };
                var result = _lua.CallFunction("hotel_tax_rules.lua", "calculate_igv_hotel", luaArgs);
                if (result.Length > 0 && result[0] is LuaTable tabla)
                {
                    tasaItem = Convert.ToDecimal(tabla["tasa"]);
                    igvItem = Convert.ToDecimal(tabla["monto"]);
                }
            }
            catch { /* usar valores por defecto */ }

            igvTotal += igvItem;

            var itemVenta = new ItemVenta
            {
                IdProducto = producto.IdProducto,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.PrecioUnitario,
                Subtotal = montoSinIgvItem + igvItem  // Subtotal con IGV incluido
            };
            itemsVenta.Add(itemVenta);
        }

        decimal montoTotal = montoSinIgvTotal + igvTotal;

        // 3. Crear la venta
        var venta = new Venta
        {
            IdCliente = cliente?.IdCliente,
            IdUsuario = idUsuario ?? 0,
            FechaVenta = DateTime.UtcNow,
            Total = montoTotal,
            MetodoPago = dto.MetodoPago,
            ItemsVenta = itemsVenta
        };
        _db.Ventas.Add(venta);
        await _db.SaveChangesAsync();  // Guardamos para obtener el IdVenta

        // 4. Generar comprobante electrónico
        var comprobante = new Comprobante
        {
            IdVenta = venta.IdVenta,
            TipoComprobante = "03",
            Serie = "B001",
            Correlativo = await ObtenerSiguienteCorrelativoAsync(),
            FechaEmision = DateTime.UtcNow,
            MontoTotal = montoTotal,
            IgvMonto = igvTotal,
            ClienteDocumentoTipo = cliente?.TipoDocumento,
            ClienteDocumentoNum = cliente?.Documento,
            ClienteNombre = cliente != null ? $"{cliente.Nombres} {cliente.Apellidos}" : "CLIENTE ANÓNIMO",
            MetodoPago = dto.MetodoPago,
            IdEstadoSunat = 1
        };
        _db.Comprobantes.Add(comprobante);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(venta.IdVenta)
            ?? throw new BusinessRuleViolationException(BusinessErrorCode.ValidationError, "Error al recuperar la venta creada.");
    }

    private async Task<int> ObtenerSiguienteCorrelativoAsync()
    {
        int ultimo = await _db.Comprobantes
            .Where(c => c.Serie == "B001")
            .MaxAsync(c => (int?)c.Correlativo) ?? 0;
        return ultimo + 1;
    }

    public async Task<PagedResult<VentaResponseDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.ItemsVenta).ThenInclude(i => i.Producto)
            .AsNoTracking()
            .OrderByDescending(v => v.FechaVenta)
            .Select(v => new VentaResponseDto
            {
                IdVenta = v.IdVenta,
                IdCliente = v.IdCliente,
                ClienteNombre = v.Cliente != null
                    ? $"{v.Cliente.Nombres} {v.Cliente.Apellidos}"
                    : "CLIENTE ANÓNIMO",
                FechaVenta = v.FechaVenta ?? DateTime.MinValue,
                Total = v.Total,
                MetodoPago = v.MetodoPago,
                Items = v.ItemsVenta.Select(i => new ItemVentaResponseDto
                {
                    IdItem = i.IdItem,
                    IdProducto = i.IdProducto,
                    NombreProducto = i.Producto != null ? i.Producto.Nombre : "",
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario,
                    Subtotal = i.Subtotal ?? 0
                }).ToList()
            });

        var paged = await query.ToPagedResultAsync(page, pageSize);
        return paged;
    }
}