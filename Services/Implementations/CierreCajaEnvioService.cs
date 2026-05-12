using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Models;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Hubs;

namespace HotelGenericoApi.Services.Implementations
{
    public class CierreCajaEnvioService : ICierreCajaEnvioService
    {
        private readonly HotelDbContext _db;
        private readonly IHubContext<HabitacionHub> _hubContext;

        public CierreCajaEnvioService(HotelDbContext db, IHubContext<HabitacionHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
        }

        public async Task<CierreCajaEnvioDto> GetEstadoAsync(DateOnly fecha)
        {
            var envio = await _db.CierreCajaEnvios
                .Include(e => e.EstadoSunat)
                .FirstOrDefaultAsync(e => e.Fecha == fecha);

            if (envio == null)
            {
                return new CierreCajaEnvioDto(fecha, 1, "Pendiente", null, 0);
            }

            return new CierreCajaEnvioDto(
                envio.Fecha,
                envio.IdEstadoSunat,
                envio.EstadoSunat?.Descripcion ?? "Desconocido",
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
                    IdEstadoSunat = 2,
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

            await _hubContext.Clients.All.SendAsync("CierreCajaEnviado", new { fecha });

            return true;
        }
    }
}