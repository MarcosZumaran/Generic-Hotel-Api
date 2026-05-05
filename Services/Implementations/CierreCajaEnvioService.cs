using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Models;
using HotelGenericoApi.Services.Interfaces;

namespace HotelGenericoApi.Services.Implementations
{
    public class CierreCajaEnvioService : ICierreCajaEnvioService
    {
        private readonly HotelDbContext _db;

        public CierreCajaEnvioService(HotelDbContext db)
        {
            _db = db;
        }

        public async Task<CierreCajaEnvioDto> GetEstadoAsync(DateOnly fecha)
        {
            var envio = await _db.CierreCajaEnvios
                .Include(e => e.IdEstadoSunatNavigation)
                .FirstOrDefaultAsync(e => e.Fecha == fecha);

            if (envio == null)
            {
                return new CierreCajaEnvioDto(fecha, 1, "Pendiente", null, 0);
            }

            return new CierreCajaEnvioDto(
                envio.Fecha,
                envio.IdEstadoSunat,
                envio.IdEstadoSunatNavigation?.Descripcion ?? "Desconocido",
                envio.FechaEnvio,
                envio.IntentosEnvio
            );
        }

        public async Task<bool> MarcarComoEnviadoAsync(DateOnly fecha)
        {
            var envio = await _db.CierreCajaEnvios.FindAsync(fecha);

            if (envio == null)
            {
                envio = new CierreCajaEnvio
                {
                    Fecha = fecha,
                    IdEstadoSunat = 2, // Enviado
                    FechaEnvio = DateTime.UtcNow,
                    IntentosEnvio = 1,
                    HashXml = "hash_simulado"
                };
                _db.CierreCajaEnvios.Add(envio);
            }
            else
            {
                envio.IdEstadoSunat = 2;
                envio.FechaEnvio = DateTime.UtcNow;
                envio.IntentosEnvio = (envio.IntentosEnvio ?? 0) + 1;
                envio.HashXml = "hash_simulado";
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}