using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Extensions;
using HotelGenericoApi.Hubs;

namespace HotelGenericoApi.Services.Implementations;

public class ComprobanteService : IComprobanteService
{
    private readonly HotelDbContext _db;
    private readonly IHubContext<HabitacionHub> _hubContext;

    public ComprobanteService(HotelDbContext db, IHubContext<HabitacionHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<ComprobanteResponseDto>> GetAllAsync()
    {
        return await _db.Comprobantes
            .Include(c => c.EstadoSunat)
            .AsNoTracking()
            .Select(c => new ComprobanteResponseDto(
                c.IdComprobante, c.IdEstancia, c.IdVenta,
                c.TipoComprobante, c.Serie, c.Correlativo,
                c.FechaEmision, c.MontoTotal, c.IgvMonto,
                c.ClienteDocumentoTipo, c.ClienteDocumentoNum,
                c.ClienteNombre, c.MetodoPago,
                c.IdEstadoSunat,
                c.EstadoSunat != null ? c.EstadoSunat.Descripcion : null,
                c.FechaEnvio, c.IntentosEnvio
            )).ToListAsync();
    }

    public async Task<ComprobanteResponseDto?> GetByIdAsync(int id)
    {
        var c = await _db.Comprobantes
            .Include(c => c.EstadoSunat)
            .FirstOrDefaultAsync(x => x.IdComprobante == id);

        if (c is null) return null;

        return new ComprobanteResponseDto(
            c.IdComprobante, c.IdEstancia, c.IdVenta,
            c.TipoComprobante, c.Serie, c.Correlativo,
            c.FechaEmision, c.MontoTotal, c.IgvMonto,
            c.ClienteDocumentoTipo, c.ClienteDocumentoNum,
            c.ClienteNombre, c.MetodoPago,
            c.IdEstadoSunat, c.EstadoSunat?.Descripcion,
            c.FechaEnvio, c.IntentosEnvio
        );
    }

    public async Task<bool> MarcarComoEnviadoAsync(int id, string hashXml)
    {
        var entity = await _db.Comprobantes.FindAsync(id);
        if (entity is null) return false;

        entity.IdEstadoSunat = 2;
        entity.FechaEnvio = DateTime.UtcNow;
        entity.IntentosEnvio = (entity.IntentosEnvio ?? 0) + 1;
        entity.HashXml = hashXml;
        await _db.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ComprobanteEmitido", new
        {
            idComprobante = entity.IdComprobante,
            tipo = entity.TipoComprobante == "03" ? "Boleta" : "Factura",
            monto = entity.MontoTotal,
            fecha = entity.FechaEmision
        });

        return true;
    }

    public async Task<PagedResult<ComprobanteResponseDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = _db.Comprobantes
            .Include(c => c.EstadoSunat)
            .AsNoTracking()
            .Select(c => new ComprobanteResponseDto(
                c.IdComprobante, c.IdEstancia, c.IdVenta,
                c.TipoComprobante, c.Serie, c.Correlativo,
                c.FechaEmision, c.MontoTotal, c.IgvMonto,
                c.ClienteDocumentoTipo, c.ClienteDocumentoNum,
                c.ClienteNombre, c.MetodoPago,
                c.IdEstadoSunat,
                c.EstadoSunat != null ? c.EstadoSunat.Descripcion : null,
                c.FechaEnvio, c.IntentosEnvio
            ));

        var paged = await query.ToPagedResultAsync(page, pageSize);
        return paged;
    }
}