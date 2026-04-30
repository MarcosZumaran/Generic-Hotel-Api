using Microsoft.EntityFrameworkCore;
using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.DTOs.Response;
using LaRicaNoche.Api.Services.Interfaces;

namespace LaRicaNoche.Api.Services.Implementations;

public class ReporteService : IReporteService
{
    private readonly LaRicaNocheDbContext _db;

    public ReporteService(LaRicaNocheDbContext db) => _db = db;

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
}