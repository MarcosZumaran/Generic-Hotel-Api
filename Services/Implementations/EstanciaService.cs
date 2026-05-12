using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Models;
using HotelGenericoApi.Models.Exceptions;
using HotelGenericoApi.Services.Interfaces;
using NLua;
using HotelGenericoApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HotelGenericoApi.Services.Implementations
{
    public class EstanciaService : IEstanciaService
    {
        private readonly HotelDbContext _db;
        private readonly ILuaService _lua;
        private readonly IDbTransactionManager _transactionManager;
        private readonly IValidadorEstadoService _validador;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<HabitacionHub> _hubContext;

        private const string ESTADO_ACTIVA = "Activa";
        private const string ESTADO_FINALIZADA = "Finalizada";
        private const string ESTADO_CONFIRMADA = "Confirmada";
        private const string ESTADO_CHECKIN_REALIZADO = "Check‑in realizado";

        // Constructor principal con todas las dependencias
        public EstanciaService(
            HotelDbContext db,
            ILuaService lua,
            IDbTransactionManager transactionManager,
            IValidadorEstadoService validador,
            Microsoft.AspNetCore.SignalR.IHubContext<HabitacionHub> hubContext
        )
        {
            _db = db;
            _lua = lua;
            _transactionManager = transactionManager;
            _validador = validador;
            _hubContext = hubContext;
        }

        // Constructor simplificado que usa las implementaciones reales
        public EstanciaService(HotelDbContext db, ILuaService lua) : this(db, lua, new SqlServerTransactionManager(db), new ValidadorEstadoService(db), null!)
        {
        }

        public async Task<IEnumerable<EstanciaResponseDto>> GetAllAsync()
        {
            return await _db.Estancias
                .Include(e => e.Habitacion)
                .Include(e => e.ClienteTitular)
                .AsNoTracking()
                .Select(e => MapToResponse(e))
                .ToListAsync();
        }

        public async Task<EstanciaResponseDto?> GetByIdAsync(int id)
        {
            var e = await _db.Estancias
                .Include(e => e.Habitacion)
                .Include(e => e.ClienteTitular)
                .FirstOrDefaultAsync(e => e.IdEstancia == id);

            return e is not null ? MapToResponse(e) : null;
        }

        public async Task<EstanciaResponseDto> CheckInAsync(CheckInDto dto, int? idUsuario)
        {
            var habitacion = await _db.Habitaciones
                .Include(h => h.Estado)
                .Include(h => h.TipoHabitacion)
                .FirstOrDefaultAsync(h => h.IdHabitacion == dto.IdHabitacion);

            if (habitacion is null) throw new BusinessRuleViolationException(BusinessErrorCode.RoomNotAvailable, "La habitación no existe.");
            if (habitacion.Estado == null || !habitacion.Estado.PermiteCheckin)
                throw new BusinessRuleViolationException(BusinessErrorCode.RoomNotAvailable, "La habitación no está disponible para check‑in.");

            // Validar reserva si se proporciona
            if (dto.IdReserva.HasValue)
            {
                var reserva = await _db.Reservas.FindAsync(dto.IdReserva.Value);
                if (reserva is null) throw new BusinessRuleViolationException(BusinessErrorCode.ReservationNotFound, "La reserva no existe.");
                if (reserva.Estado != "Confirmada") throw new BusinessRuleViolationException(BusinessErrorCode.ReservationNotFound, "La reserva no está confirmada.");
                if (reserva.IdHabitacion != dto.IdHabitacion) throw new BusinessRuleViolationException(BusinessErrorCode.ReservationConflict, "La habitación no coincide con la reserva.");
                reserva.Estado = "Check‑in realizado";
            }

            Cliente cliente;
            if (dto.UsarClienteAnonimo)
            {
                cliente = await _db.Clientes
                    .FirstOrDefaultAsync(c => c.TipoDocumento == "0" && c.Documento == "00000000")
                    ?? throw new BusinessRuleViolationException(BusinessErrorCode.ClientNotFound, "Cliente anónimo no configurado.");
            }
            else
            {
                cliente = await _db.Clientes.FirstOrDefaultAsync(
                    c => c.TipoDocumento == dto.TipoDocumento && c.Documento == dto.Documento);
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

            int noches = (int)(dto.FechaCheckoutPrevista.Date - DateTime.UtcNow.Date).TotalDays;
            if (noches < 1) noches = 1;

            var tarifa = await _db.Tarifas
                .Where(t => t.IdTipoHabitacion == habitacion.IdTipo &&
                            (t.FechaInicio == null || t.FechaInicio <= DateOnly.FromDateTime(DateTime.UtcNow)) &&
                            (t.FechaFin == null || t.FechaFin >= DateOnly.FromDateTime(DateTime.UtcNow)))
                .OrderByDescending(t => t.Temporada != null ? t.Temporada.Multiplicador : 1)
                .FirstOrDefaultAsync();

            decimal precioNoche = tarifa?.Precio ?? habitacion.PrecioNoche;
            decimal montoSinIgv = precioNoche * noches;

            var configuracion = await _db.Configuracion.FirstOrDefaultAsync();
            decimal tasaDefecto = configuracion?.TasaIgvHotel ?? 10.5m;
            decimal tasaIgv = tasaDefecto;
            decimal igvCalculado = montoSinIgv * (tasaIgv / 100);

            try
            {
                var resultado = _lua.CallFunction("hotel_tax_rules.lua", "Calculate_igv_hotel", "10", montoSinIgv, "03");
                if (resultado.Length > 0 && resultado[0] is LuaTable tabla)
                {
                    tasaIgv = Convert.ToDecimal(tabla["tasa"]);
                    igvCalculado = Convert.ToDecimal(tabla["monto"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Lua: " + ex.Message);
                throw new BusinessRuleViolationException(BusinessErrorCode.LuaExecutionError, "Error al calcular el IGV.");
            }

            decimal montoTotal = montoSinIgv + igvCalculado;

            var estancia = new Estancium
            {
                IdHabitacion = dto.IdHabitacion,
                IdClienteTitular = cliente.IdCliente,
                IdReserva = dto.IdReserva,
                FechaCheckin = DateTime.UtcNow,
                FechaCheckoutPrevista = dto.FechaCheckoutPrevista,
                MontoTotal = montoTotal,
                Estado = ESTADO_ACTIVA,
                CreatedAt = DateTime.UtcNow
            };
            _db.Estancias.Add(estancia);

            var estadoOcupada = await _db.EstadosHabitacion.FirstOrDefaultAsync(e => e.PermiteCheckout);
            if (estadoOcupada is not null)
            {
                int estadoAnterior = habitacion.IdEstado;
                bool permitida = await _validador.EsTransicionValidaAsync(estadoAnterior, estadoOcupada.IdEstado);
                if (!permitida) throw new BusinessRuleViolationException(BusinessErrorCode.InvalidTransition, "Transición de estado no permitida.");

                habitacion.IdEstado = estadoOcupada.IdEstado;
                habitacion.FechaUltimoCambio = DateTime.UtcNow;
                habitacion.UsuarioCambio = idUsuario;

                _db.HistorialesEstadoHabitacion.Add(new HistorialEstadoHabitacion
                {
                    IdHabitacion = habitacion.IdHabitacion,
                    IdEstadoAnterior = estadoAnterior,
                    IdEstadoNuevo = estadoOcupada.IdEstado,
                    FechaCambio = DateTime.UtcNow,
                    IdUsuario = idUsuario ?? 0,
                    Observacion = "Check‑In automático"
                });
            }

            await _db.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("EstadoHabitacionCambiado", new
            {
                habitacion.IdHabitacion,
                habitacion.NumeroHabitacion,
                IdEstado = estadoOcupada.IdEstado,
                habitacion.FechaUltimoCambio
            });

            var comprobante = new Comprobante
            {
                IdEstancia = estancia.IdEstancia,
                TipoComprobante = "03",
                Serie = "B001",
                Correlativo = await ObtenerSiguienteCorrelativoAsync(),
                FechaEmision = DateTime.UtcNow,
                MontoTotal = montoTotal,
                IgvMonto = igvCalculado,
                ClienteDocumentoTipo = cliente.TipoDocumento,
                ClienteDocumentoNum = cliente.Documento,
                ClienteNombre = $"{cliente.Nombres} {cliente.Apellidos}",
                MetodoPago = dto.MetodoPago,
                IdEstadoSunat = 1
            };
            _db.Comprobantes.Add(comprobante);
            await _db.SaveChangesAsync();

            await _db.Entry(estancia).Reference(e => e.Habitacion).LoadAsync();
            await _db.Entry(estancia).Reference(e => e.ClienteTitular).LoadAsync();
            return MapToResponse(estancia);
        }

        public async Task<EstanciaResponseDto> CheckOutAsync(int idEstancia, int? idUsuario)
        {
            var estancia = await _db.Estancias
                .Include(e => e.Habitacion)
                .FirstOrDefaultAsync(e => e.IdEstancia == idEstancia);

            if (estancia is null) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotFound, "La estancia no existe.");
            if (estancia.Estado != ESTADO_ACTIVA) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotActive, "La estancia no está activa.");

            estancia.Estado = ESTADO_FINALIZADA;
            estancia.FechaCheckoutReal = DateTime.UtcNow;

            var estadoLimpieza = await _db.EstadosHabitacion.FirstOrDefaultAsync(e => e.Nombre == "Limpieza");
            if (estancia.Habitacion is not null && estadoLimpieza is not null)
            {
                var habitacion = estancia.Habitacion;
                int estadoAnterior = habitacion.IdEstado;

                bool permitida = await _validador.EsTransicionValidaAsync(estadoAnterior, estadoLimpieza.IdEstado);
                if (!permitida) throw new BusinessRuleViolationException(BusinessErrorCode.InvalidTransition, "Transición de estado no permitida.");

                habitacion.IdEstado = estadoLimpieza.IdEstado;
                habitacion.FechaUltimoCambio = DateTime.UtcNow;
                habitacion.UsuarioCambio = idUsuario;

                _db.HistorialesEstadoHabitacion.Add(new HistorialEstadoHabitacion
                {
                    IdHabitacion = habitacion.IdHabitacion,
                    IdEstadoAnterior = estadoAnterior,
                    IdEstadoNuevo = estadoLimpieza.IdEstado,
                    FechaCambio = DateTime.UtcNow,
                    IdUsuario = idUsuario ?? 0,
                    Observacion = "Check‑Out automático"
                });
            }

            await _db.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("EstadoHabitacionCambiado", new
            {
                estancia.Habitacion.IdHabitacion,
                estancia.Habitacion.NumeroHabitacion,
                IdEstado = estadoLimpieza.IdEstado,
                estancia.Habitacion.FechaUltimoCambio
            });

            await _db.Entry(estancia).Reference(e => e.Habitacion).LoadAsync();
            await _db.Entry(estancia).Reference(e => e.ClienteTitular).LoadAsync();
            return MapToResponse(estancia);
        }

        private async Task<int> ObtenerSiguienteCorrelativoAsync()
        {
            int ultimo = await _db.Comprobantes
                .Where(c => c.Serie == "B001")
                .MaxAsync(c => (int?)c.Correlativo) ?? 0;
            return ultimo + 1;
        }

        private static EstanciaResponseDto MapToResponse(Estancium e)
        {
            return new EstanciaResponseDto(
                e.IdEstancia, e.IdHabitacion,
                e.Habitacion?.NumeroHabitacion,
                e.IdClienteTitular,
                e.ClienteTitular is not null
                    ? $"{e.ClienteTitular.Nombres} {e.ClienteTitular.Apellidos}"
                    : null,
                e.FechaCheckin, e.FechaCheckoutPrevista,
                e.FechaCheckoutReal, e.MontoTotal, e.Estado, e.CreatedAt);
        }

        public async Task<EstanciaResponseDto> RegistrarConsumoAsync(int idEstancia, ConsumoEstanciaCreateDto dto, int? idUsuario)
        {
            var estancia = await _db.Estancias
                .Include(e => e.Habitacion)
                .Include(e => e.ClienteTitular)
                .FirstOrDefaultAsync(e => e.IdEstancia == idEstancia);

            if (estancia is null) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotFound, "Estancia no encontrada.");
            if (estancia.Estado != ESTADO_ACTIVA) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotActive, "La estancia no está activa.");

            var producto = await _db.Productos.FindAsync(dto.IdProducto);
            if (producto is null) throw new BusinessRuleViolationException(BusinessErrorCode.ProductNotFound, "Producto no encontrado.");

            var itemEstancia = new ItemEstancium
            {
                IdEstancia = idEstancia,
                IdProducto = dto.IdProducto,
                Cantidad = dto.Cantidad,
                PrecioUnitario = producto.PrecioUnitario,
                FechaRegistro = DateTime.UtcNow
            };
            _db.ItemsEstancia.Add(itemEstancia);
            await _db.SaveChangesAsync();

            await _db.Entry(itemEstancia).ReloadAsync();
            decimal subtotal = itemEstancia.Subtotal ?? (producto.PrecioUnitario * dto.Cantidad);
            estancia.MontoTotal += subtotal;
            await _db.SaveChangesAsync();

            var comprobante = await _db.Comprobantes.FirstOrDefaultAsync(c => c.IdEstancia == estancia.IdEstancia);
            if (comprobante is not null)
            {
                comprobante.MontoTotal = estancia.MontoTotal;
                await _db.SaveChangesAsync();
            }

            return MapToResponse(estancia);
        }

        public async Task<ReservaResponseDto> CrearReservaAsync(ReservaCreateDto dto, int? idUsuario)
        {
            await _transactionManager.BeginTransactionAsync();
            try
            {
                var habitacion = await _db.Habitaciones
                    .Include(h => h.Estado)
                    .Include(h => h.TipoHabitacion)
                    .FirstOrDefaultAsync(h => h.IdHabitacion == dto.IdHabitacion);

                if (habitacion is null) throw new BusinessRuleViolationException(BusinessErrorCode.RoomNotAvailable, "La habitación no existe.");
                if (habitacion.Estado == null || !habitacion.Estado.PermiteCheckin)
                    throw new BusinessRuleViolationException(BusinessErrorCode.RoomNotAvailable, "La habitación no está disponible para reserva.");

                var conflicto = await _db.Reservas.AnyAsync(r =>
                    r.IdHabitacion == dto.IdHabitacion && r.Estado != "Cancelada" &&
                    r.FechaEntradaPrevista < dto.FechaSalidaPrevista && r.FechaSalidaPrevista > dto.FechaEntradaPrevista);
                if (conflicto) throw new BusinessRuleViolationException(BusinessErrorCode.ReservationConflict, "La habitación ya está reservada en ese rango de fechas.");

                Cliente cliente;
                if (dto.UsarClienteAnonimo)
                {
                    cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.TipoDocumento == "0" && c.Documento == "00000000")
                        ?? throw new BusinessRuleViolationException(BusinessErrorCode.ClientNotFound, "Cliente anónimo no configurado.");
                }
                else
                {
                    cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.TipoDocumento == dto.TipoDocumento && c.Documento == dto.Documento);
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

                int noches = (int)(dto.FechaSalidaPrevista.Date - dto.FechaEntradaPrevista.Date).TotalDays;
                if (noches < 1) noches = 1;

                var tarifa = await _db.Tarifas
                    .Where(t => t.IdTipoHabitacion == habitacion.IdTipo &&
                                (t.FechaInicio == null || t.FechaInicio <= DateOnly.FromDateTime(dto.FechaEntradaPrevista)) &&
                                (t.FechaFin == null || t.FechaFin >= DateOnly.FromDateTime(dto.FechaEntradaPrevista)))
                    .OrderByDescending(t => t.Temporada != null ? t.Temporada.Multiplicador : 1)
                    .FirstOrDefaultAsync();

                decimal precioNoche = tarifa?.Precio ?? habitacion.PrecioNoche;
                decimal montoSinIgv = precioNoche * noches;

                var configuracion = await _db.Configuracion.FirstOrDefaultAsync();
                decimal tasaDefecto = configuracion?.TasaIgvHotel ?? 10.5m;
                decimal tasaIgv = tasaDefecto;
                decimal igvCalculado = montoSinIgv * (tasaIgv / 100);

                try
                {
                    var resultado = _lua.CallFunction("hotel_tax_rules.lua", "Calculate_igv_hotel", "10", montoSinIgv, "03");
                    if (resultado.Length > 0 && resultado[0] is LuaTable tabla)
                    {
                        tasaIgv = Convert.ToDecimal(tabla["tasa"]);
                        igvCalculado = Convert.ToDecimal(tabla["monto"]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Lua: " + ex.Message);
                    throw new BusinessRuleViolationException(BusinessErrorCode.LuaExecutionError, "Error al calcular el IGV.");
                }

                decimal montoTotal = montoSinIgv + igvCalculado;

                var reserva = new Reserva
                {
                    IdHabitacion = dto.IdHabitacion,
                    IdCliente = cliente.IdCliente,
                    IdUsuario = idUsuario ?? 0,
                    FechaRegistro = DateTime.UtcNow,
                    FechaEntradaPrevista = dto.FechaEntradaPrevista,
                    FechaSalidaPrevista = dto.FechaSalidaPrevista,
                    MontoTotal = montoTotal,
                    Estado = ESTADO_CONFIRMADA,
                    Observaciones = $"Reserva creada para {noches} noche(s)."
                };
                _db.Reservas.Add(reserva);

                if (dto.FechaEntradaPrevista.Date == DateTime.UtcNow.Date)
                {
                    var estadoOcupada = await _db.EstadosHabitacion.FirstOrDefaultAsync(e => e.PermiteCheckout);
                    if (estadoOcupada is not null)
                    {
int estadoAnterior = habitacion.IdEstado;
                        bool permitida = await _validador.EsTransicionValidaAsync(estadoAnterior, estadoOcupada.IdEstado);
                        if (!permitida) throw new BusinessRuleViolationException(BusinessErrorCode.InvalidTransition, "Transición de estado no permitida.");

                        habitacion.IdEstado = estadoOcupada.IdEstado;
                        habitacion.FechaUltimoCambio = DateTime.UtcNow;
                        habitacion.UsuarioCambio = idUsuario;

                        _db.HistorialesEstadoHabitacion.Add(new HistorialEstadoHabitacion
                        {
                            IdHabitacion = habitacion.IdHabitacion,
                            IdEstadoAnterior = estadoAnterior,
                            IdEstadoNuevo = estadoOcupada.IdEstado,
                            FechaCambio = DateTime.UtcNow,
                            IdUsuario = idUsuario ?? 0,
                            Observacion = "Reserva con entrada hoy — cambio automático a Ocupada"
                        });
                    }
                }

                await _db.SaveChangesAsync();
                await _transactionManager.CommitAsync();

                return new ReservaResponseDto(
                    reserva.IdReserva, reserva.IdHabitacion,
                    habitacion.NumeroHabitacion, $"{cliente.Nombres} {cliente.Apellidos}",
                    reserva.FechaEntradaPrevista, reserva.FechaSalidaPrevista,
                    reserva.MontoTotal, reserva.Estado ?? ESTADO_CONFIRMADA);
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }
            finally
            {
                await _transactionManager.DisposeAsync();
            }
        }

        public async Task<ReservaResponseDto?> GetReservaByIdAsync(int id)
        {
            var reserva = await _db.Reservas
                .Include(r => r.Habitacion)
                .Include(r => r.Cliente)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdReserva == id);
            if (reserva is null) return null;
            return new ReservaResponseDto(
                reserva.IdReserva, reserva.IdHabitacion,
                reserva.Habitacion?.NumeroHabitacion,
                reserva.Cliente != null
                    ? $"{reserva.Cliente.Nombres} {reserva.Cliente.Apellidos}" : null,
                reserva.FechaEntradaPrevista, reserva.FechaSalidaPrevista,
                reserva.MontoTotal, reserva.Estado ?? "Desconocido");
        }

        public async Task<IEnumerable<ReservaResponseDto>> GetReservasPorHabitacionAsync(int idHabitacion)
        {
            return await _db.Reservas
                .Where(r => r.IdHabitacion == idHabitacion)
                .Include(r => r.Habitacion)
                .Include(r => r.Cliente)
                .AsNoTracking()
                .Select(r => new ReservaResponseDto(
                    r.IdReserva, r.IdHabitacion,
                    r.Habitacion != null ? r.Habitacion.NumeroHabitacion : null,
                    r.Cliente != null
                        ? $"{r.Cliente.Nombres} {r.Cliente.Apellidos}" : null,
                    r.FechaEntradaPrevista, r.FechaSalidaPrevista, r.MontoTotal,
                    r.Estado ?? "Pendiente"))
                .ToListAsync();
        }

        public async Task<IEnumerable<ItemConsumoResponseDto>> GetConsumosAsync(int idEstancia)
        {
            return await _db.ItemsEstancia
                .Where(i => i.IdEstancia == idEstancia)
                .Include(i => i.Producto)
                .OrderByDescending(i => i.FechaRegistro)
                .Select(i => new ItemConsumoResponseDto(
                    i.IdItem, i.IdProducto,
                    i.Producto != null ? i.Producto.Nombre : "",
                    i.Cantidad, i.PrecioUnitario, i.Subtotal ?? 0, i.FechaRegistro))
                .ToListAsync();
        }

        public async Task ActualizarConsumoAsync(int idEstancia, int idItem, int nuevaCantidad, int? idUsuario)
        {
            if (nuevaCantidad < 1) throw new BusinessRuleViolationException(BusinessErrorCode.QuantityInvalid, "La cantidad debe ser mayor a 0.");
            var estancia = await _db.Estancias.FindAsync(idEstancia) ?? throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotFound, "Estancia no encontrada.");
            if (estancia.Estado != ESTADO_ACTIVA) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotActive, "La estancia no está activa.");
            var item = await _db.ItemsEstancia.FindAsync(idItem) ?? throw new BusinessRuleViolationException(BusinessErrorCode.ProductNotFound, "Item no encontrado.");
            if (item.IdEstancia != idEstancia) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotActive, "El item no pertenece a esta estancia.");

            decimal subtotalAnterior = item.Subtotal ?? (item.PrecioUnitario * item.Cantidad);
            item.Cantidad = nuevaCantidad;
            await _db.SaveChangesAsync();
            await _db.Entry(item).ReloadAsync();
            decimal nuevoSubtotal = item.Subtotal ?? (item.PrecioUnitario * nuevaCantidad);
            estancia.MontoTotal = estancia.MontoTotal - subtotalAnterior + nuevoSubtotal;

            var comprobante = await _db.Comprobantes.FirstOrDefaultAsync(c => c.IdEstancia == idEstancia);
            if (comprobante is not null)
            {
                comprobante.MontoTotal = estancia.MontoTotal;
            }
            await _db.SaveChangesAsync();
        }

        public async Task EliminarConsumoAsync(int idEstancia, int idItem, int? idUsuario)
        {
            var estancia = await _db.Estancias.FindAsync(idEstancia) ?? throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotFound, "Estancia no encontrada.");
            if (estancia.Estado != ESTADO_ACTIVA) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotActive, "La estancia no está activa.");
            var item = await _db.ItemsEstancia.FindAsync(idItem) ?? throw new BusinessRuleViolationException(BusinessErrorCode.ProductNotFound, "Item no encontrado.");
            if (item.IdEstancia != idEstancia) throw new BusinessRuleViolationException(BusinessErrorCode.EstanciaNotActive, "El item no pertenece a esta estancia.");

            decimal subtotal = item.Subtotal ?? (item.PrecioUnitario * item.Cantidad);
            _db.ItemsEstancia.Remove(item);
            estancia.MontoTotal -= subtotal;

            var comprobante = await _db.Comprobantes.FirstOrDefaultAsync(c => c.IdEstancia == idEstancia);
            if (comprobante is not null)
            {
                comprobante.MontoTotal = estancia.MontoTotal;
            }
            await _db.SaveChangesAsync();
        }
    }
}