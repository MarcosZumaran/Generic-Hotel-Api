using Microsoft.EntityFrameworkCore;
using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.DTOs.Response;
using LaRicaNoche.Api.Models;
using LaRicaNoche.Api.Services.Interfaces;
using NLua;

namespace LaRicaNoche.Api.Services.Implementations;

public class EstanciaService : IEstanciaService
{
    private readonly LaRicaNocheDbContext _db;
    private readonly ILuaService _lua;

    public EstanciaService(LaRicaNocheDbContext db, ILuaService lua)
    {
        _db = db;
        _lua = lua;
    }

    public async Task<IEnumerable<EstanciaResponseDto>> GetAllAsync()
    {
        return await _db.Estancias
            .Include(e => e.IdHabitacionNavigation)
            .Include(e => e.IdClienteTitularNavigation)
            .AsNoTracking()
            .Select(e => MapToResponse(e))
            .ToListAsync();
    }

    public async Task<EstanciaResponseDto?> GetByIdAsync(int id)
    {
        var e = await _db.Estancias
            .Include(e => e.IdHabitacionNavigation)
            .Include(e => e.IdClienteTitularNavigation)
            .FirstOrDefaultAsync(e => e.IdEstancia == id);

        return e is not null ? MapToResponse(e) : null;
    }

    public async Task<EstanciaResponseDto> CheckInAsync(CheckInDto dto, int? idUsuario)
    {
        // 1. Verificar que la habitación esté disponible
        var habitacion = await _db.Habitaciones
            .Include(h => h.IdEstadoNavigation)
            .Include(h => h.IdTipoNavigation)
            .FirstOrDefaultAsync(h => h.IdHabitacion == dto.IdHabitacion);

        if (habitacion is null) throw new InvalidOperationException("La habitación no existe.");

        if (habitacion.IdEstadoNavigation?.Nombre != "Disponible") throw new InvalidOperationException("La habitación no está disponible.");

        // 2. Obtener o crear cliente
        Cliente cliente;
        if (dto.UsarClienteAnonimo)
        {
            cliente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.TipoDocumento == "0" && c.Documento == "00000000")
                ?? throw new InvalidOperationException("Cliente anónimo no configurado en el sistema.");
        }
        else
        {
            cliente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.TipoDocumento == dto.TipoDocumento && c.Documento == dto.Documento);

            if (cliente is null)
            {
                cliente = new Cliente
                {
                    TipoDocumento = dto.TipoDocumento,
                    Documento = dto.Documento,
                    Nombres = dto.Nombres,
                    Apellidos = dto.Apellidos,
                    Telefono = dto.Telefono,
                    FechaRegistro = DateTime.UtcNow
                };
                _db.Clientes.Add(cliente);
                await _db.SaveChangesAsync();
            }
        }

        // 3. Calcular noches y monto sin IGV
        int noches = (int)(dto.FechaCheckoutPrevista.Date - DateTime.UtcNow.Date).TotalDays;
        if (noches < 1) noches = 1;

        decimal montoSinIgv = habitacion.PrecioNoche * noches;

        // 4. Ejecutar script Lua para calcular IGV hotelero
        decimal tasaIgv = 10.5m; // valor por defecto
        decimal igvCalculado = montoSinIgv * (tasaIgv / 100);
        try
        {
            // Llamar al script Lua "hotel_tax_rules.lua", función "Calculate_igv_hotel"
            var resultado = _lua.CallFunction("hotel_tax_rules.lua", "Calculate_igv_hotel",
                "10", montoSinIgv, "03");

            // El resultado es object[]; el primer elemento es una LuaTable
            if (resultado.Length > 0 && resultado[0] is LuaTable tabla)
            {
                tasaIgv = Convert.ToDecimal(tabla["tasa"]);
                igvCalculado = Convert.ToDecimal(tabla["monto"]);
            }
        }
        catch (Exception ex)
        {
            // Manejar la excepción
            Console.WriteLine("Error al llamar al script Lua: " + ex.Message);
            throw new InvalidOperationException("Error al calcular el IGV: " + ex.Message);
        }

        decimal montoTotal = montoSinIgv + igvCalculado;

        // 5. Crear estancia
        var estancia = new Estancia
        {
            IdHabitacion = dto.IdHabitacion,
            IdClienteTitular = cliente.IdCliente,
            FechaCheckin = DateTime.UtcNow,
            FechaCheckoutPrevista = dto.FechaCheckoutPrevista,
            MontoTotal = montoTotal,
            Estado = "Activa",
            CreatedAt = DateTime.UtcNow
        };
        _db.Estancias.Add(estancia);

        // 6. Cambiar estado de la habitación a "Ocupada"
        var estadoOcupada = await _db.CatEstadoHabitacions
            .FirstOrDefaultAsync(es => es.Nombre == "Ocupada");
        if (estadoOcupada is not null)
        {
            habitacion.IdEstado = estadoOcupada.IdEstado;
            habitacion.FechaUltimoCambio = DateTime.UtcNow;
            habitacion.UsuarioCambio = idUsuario;
        }

        // 7. Registrar en el historial de estados
        var historial = new HistorialEstadoHabitacion
        {
            IdHabitacion = habitacion.IdHabitacion,
            IdEstadoAnterior = 1, // Disponible (asumimos el ID 1)
            IdEstadoNuevo = estadoOcupada?.IdEstado ?? 2,
            FechaCambio = DateTime.UtcNow,
            IdUsuario = idUsuario,
            Observacion = "Check-In automático"
        };
        _db.HistorialEstadoHabitacions.Add(historial);

        await _db.SaveChangesAsync();

        // 8. Generar comprobante
        var comprobante = new Comprobante
        {
            IdEstancia = estancia.IdEstancia,
            TipoComprobante = "03", // Boleta
            Serie = "B001",
            Correlativo = await ObtenerSiguienteCorrelativoAsync(),
            FechaEmision = DateTime.UtcNow,
            MontoTotal = montoTotal,
            IgvMonto = igvCalculado,
            ClienteDocumentoTipo = cliente.TipoDocumento,
            ClienteDocumentoNum = cliente.Documento,
            ClienteNombre = $"{cliente.Nombres} {cliente.Apellidos}",
            MetodoPago = dto.MetodoPago,
            IdEstadoSunat = 1 // Pendiente
        };
        _db.Comprobantes.Add(comprobante);
        await _db.SaveChangesAsync();

        // Recargar navegaciones para el DTO
        await _db.Entry(estancia).Reference(e => e.IdHabitacionNavigation).LoadAsync();
        await _db.Entry(estancia).Reference(e => e.IdClienteTitularNavigation).LoadAsync();

        return MapToResponse(estancia);
    }

    public async Task<EstanciaResponseDto> CheckOutAsync(int idEstancia, int? idUsuario)
    {
        var estancia = await _db.Estancias
            .Include(e => e.IdHabitacionNavigation)
            .FirstOrDefaultAsync(e => e.IdEstancia == idEstancia);

        if (estancia is null)
            throw new InvalidOperationException("La estancia no existe.");

        if (estancia.Estado != "Activa")
            throw new InvalidOperationException("La estancia no está activa.");

        // 1. Cambiar estado de la estancia
        estancia.Estado = "Finalizada";
        estancia.FechaCheckoutReal = DateTime.UtcNow;

        // 2. Cambiar estado de la habitación a Limpieza
        var estadoLimpieza = await _db.CatEstadoHabitacions
            .FirstOrDefaultAsync(es => es.Nombre == "Limpieza");

        if (estancia.IdHabitacionNavigation is not null && estadoLimpieza is not null)
        {
            var habitacion = estancia.IdHabitacionNavigation;
            int estadoAnterior = habitacion.IdEstado ?? 0;

            habitacion.IdEstado = estadoLimpieza.IdEstado;
            habitacion.FechaUltimoCambio = DateTime.UtcNow;
            habitacion.UsuarioCambio = idUsuario;

            // 3. Registrar en historial
            var historial = new HistorialEstadoHabitacion
            {
                IdHabitacion = habitacion.IdHabitacion,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = estadoLimpieza.IdEstado,
                FechaCambio = DateTime.UtcNow,
                IdUsuario = idUsuario,
                Observacion = "Check-Out automático"
            };
            _db.HistorialEstadoHabitacions.Add(historial);
        }

        await _db.SaveChangesAsync();

        // Recargar navegaciones
        await _db.Entry(estancia).Reference(e => e.IdHabitacionNavigation).LoadAsync();
        await _db.Entry(estancia).Reference(e => e.IdClienteTitularNavigation).LoadAsync();

        return MapToResponse(estancia);
    }

    private async Task<int> ObtenerSiguienteCorrelativoAsync()
    {
        int ultimo = await _db.Comprobantes
            .Where(c => c.Serie == "B001")
            .MaxAsync(c => (int?)c.Correlativo) ?? 0;
        return ultimo + 1;
    }

    private static EstanciaResponseDto MapToResponse(Estancia e)
    {
        return new EstanciaResponseDto(
            e.IdEstancia,
            e.IdHabitacion,
            e.IdHabitacionNavigation?.NumeroHabitacion,
            e.IdClienteTitular,
            e.IdClienteTitularNavigation is not null
                ? $"{e.IdClienteTitularNavigation.Nombres} {e.IdClienteTitularNavigation.Apellidos}"
                : null,
            e.FechaCheckin,
            e.FechaCheckoutPrevista,
            e.FechaCheckoutReal,
            e.MontoTotal,
            e.Estado,
            e.CreatedAt
        );
    }
}