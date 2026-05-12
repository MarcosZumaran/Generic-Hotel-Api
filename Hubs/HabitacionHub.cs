using Microsoft.AspNetCore.SignalR;

namespace HotelGenericoApi.Hubs;

public class HabitacionHub : Hub
{
    public async Task NotificarComprobanteEmitido(int idComprobante, string tipo, decimal monto, DateTime fecha)
    {
        await Clients.All.SendAsync("ComprobanteEmitido", new
        {
            idComprobante,
            tipo,
            monto,
            fecha
        });
    }

    public async Task NotificarCierreCajaEnviado(DateTime fecha)
    {
        await Clients.All.SendAsync("CierreCajaEnviado", new { fecha });
    }

    public async Task NotificarCambioEstadoHabitacion(int idHabitacion, string numero, string nuevoEstado)
    {
        await Clients.All.SendAsync("EstadoHabitacionCambiado", new
        {
            idHabitacion,
            numero,
            nuevoEstado
        });
    }

    public async Task NotificarNuevaEstancia(int idEstancia, string numeroHabitacion, string cliente)
    {
        await Clients.All.SendAsync("NuevaEstancia", new
        {
            idEstancia,
            numeroHabitacion,
            cliente
        });
    }
}