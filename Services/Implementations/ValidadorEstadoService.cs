using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations;

public class ValidadorEstadoService : IValidadorEstadoService
{
    private readonly HotelDbContext _db;

    public ValidadorEstadoService(HotelDbContext db)
    {
        _db = db;
    }

    public async Task<bool> EsTransicionValidaAsync(int idEstadoActual, int idEstadoSiguiente)
    {
        return await _db.TransicionesEstado.AnyAsync(t =>
            t.IdEstadoActual == idEstadoActual && t.IdEstadoSiguiente == idEstadoSiguiente);
    }
}