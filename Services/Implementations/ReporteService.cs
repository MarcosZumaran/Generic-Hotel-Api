using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class ReporteService : IReporteService
{
    private readonly HotelDbContext _db;

    public ReporteService(HotelDbContext db) => _db = db;

    public async Task<IEnumerable<CierreCajaResponseDto>> GetCierreCajaAsync(DateOnly? fecha)
    {
        var query = _db.VCierreCajaDiarios.AsNoTracking();
        if (fecha.HasValue)
            query = query.Where(v => v.Fecha == fecha.Value);

        return await query.Select(v => new CierreCajaResponseDto(
            v.Fecha, v.MetodoPago, v.Ingresos, v.Concepto
        )).ToListAsync();
    }

    public async Task<IEnumerable<EstadoHabitacionResponseDto>> GetEstadoHabitacionesAsync()
    {
        return await _db.VEstadoHabitaciones
            .AsNoTracking()
            .Select(v => new EstadoHabitacionResponseDto(
                v.NumeroHabitacion, v.TipoHabitacion, v.Estado,
                v.PrecioNoche, v.FechaUltimoCambio
            )).ToListAsync();
    }

    public async Task<byte[]> ExportarCierreCajaExcelAsync(DateOnly? fecha)
    {
        var datos = await GetCierreCajaAsync(fecha);
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add("Cierre de Caja");

        ws.Cell(1, 1).Value = "Cierre de Caja";
        ws.Range(1, 1, 1, 3).Merge().Style.Font.Bold = true;

        ws.Cell(2, 1).Value = "Concepto";
        ws.Cell(2, 2).Value = "Método de Pago";
        ws.Cell(2, 3).Value = "Ingresos";

        int row = 3;
        foreach (var item in datos)
        {
            ws.Cell(row, 1).Value = item.Concepto;
            ws.Cell(row, 2).Value = item.MetodoPago;
            ws.Cell(row, 3).Value = item.Ingresos;
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportarEstadoHabitacionesExcelAsync()
    {
        var datos = await GetEstadoHabitacionesAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add("Estado Habitaciones");

        ws.Cell(1, 1).Value = "N°";
        ws.Cell(1, 2).Value = "Tipo";
        ws.Cell(1, 3).Value = "Estado";
        ws.Cell(1, 4).Value = "Precio/Noche";
        ws.Cell(1, 5).Value = "Último Cambio";

        int row = 2;
        foreach (var item in datos)
        {
            ws.Cell(row, 1).Value = item.NumeroHabitacion;
            ws.Cell(row, 2).Value = item.TipoHabitacion;
            ws.Cell(row, 3).Value = item.Estado;
            ws.Cell(row, 4).Value = item.PrecioNoche;
            ws.Cell(row, 5).Value = item.FechaUltimoCambio?.ToString("dd/MM/yyyy HH:mm");
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<IEnumerable<TopProductoDto>> GetTopProductosAsync(int dias = 30)
    {
        var fechaLimite = DateTime.UtcNow.AddDays(-dias);

        return await _db.ItemsEstancia
            .Where(i => i.FechaRegistro >= fechaLimite)
            .GroupBy(i => i.IdProductoNavigation.Nombre)
            .Select(g => new TopProductoDto
            {
                Nombre = g.Key,
                CantidadTotal = g.Sum(i => i.Cantidad),
                IngresoTotal = g.Sum(i => i.Subtotal ?? 0)
            })
            .OrderByDescending(t => t.CantidadTotal)
            .Take(5)
            .ToListAsync();
    }
}