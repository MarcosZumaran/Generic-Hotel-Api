using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NLua;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.Hubs;
using HotelGenericoApi.Models;
using HotelGenericoApi.Services.Implementations;
using HotelGenericoApi.Services.Interfaces;
using Xunit;

namespace HotelGenericoApi.Tests;

public class EstanciaServiceTests
{
    private HotelDbContext CreateContext() => TestDbContextFactory.Create();

    private class NoOpClientProxy : IClientProxy
    {
        public Task SendCoreAsync(string method, object?[]? args, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private static IHubContext<HabitacionHub> CreateMockHubContext()
    {
        var mockHubClients = new Mock<IHubClients>();
        mockHubClients.Setup(c => c.All).Returns(new NoOpClientProxy());

        var mockHubContext = new Mock<IHubContext<HabitacionHub>>();
        mockHubContext.Setup(h => h.Clients).Returns(mockHubClients.Object);

        return mockHubContext.Object;
    }

    private ILuaService CreateMockLuaService(decimal tasa = 10.5m)
    {
        var mock = new Mock<ILuaService>();
        mock.Setup(l => l.CallFunction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>()))
            .Returns((string script, string func, object[] args) =>
            {
                var montoSinIgv = (decimal)args[1];
                var montoIgv = montoSinIgv * (tasa / 100);
                return new object[] { new Dictionary<string, object> { ["tasa"] = tasa, ["monto"] = montoIgv } };
            });
        return mock.Object;
    }

    [Fact]
    public async Task CheckIn_HabitacionDisponible_CreaEstanciaCorrectamente()
    {
        var db = CreateContext();
        var luaMock = CreateMockLuaService();
        var txManager = TestTransactionFactory.Create();
        var validador = new ValidadorEstadoService(db);
        var service = new EstanciaService(db, luaMock, txManager, validador, CreateMockHubContext());

        var dto = new CheckInDto
        {
            IdHabitacion = 1,
            TipoDocumento = "1",
            Documento = "61077298",
            Nombres = "Test",
            Apellidos = "Cliente",
            FechaCheckoutPrevista = DateTime.UtcNow.AddDays(2),
            MetodoPago = "005"
        };

        var result = await service.CheckInAsync(dto, 1);

        Assert.NotNull(result);
        Assert.Equal(1, result.IdHabitacion);
        Assert.Equal("Activa", result.Estado);

        var habitacion = await db.Habitaciones.FindAsync(1);
        Assert.NotNull(habitacion);
        var estado = await db.EstadosHabitacion.FindAsync(habitacion.IdEstado);
        Assert.True(estado?.PermiteCheckout);
    }

    [Fact]
    public async Task CheckIn_HabitacionOcupada_LanzaExcepcion()
    {
        var db = CreateContext();
        var luaMock = CreateMockLuaService();
        var txManager = TestTransactionFactory.Create();
        var validador = new ValidadorEstadoService(db);
        var service = new EstanciaService(db, luaMock, txManager, validador, CreateMockHubContext());

        var dto = new CheckInDto
        {
            IdHabitacion = 2,
            TipoDocumento = "1",
            Documento = "61077298",
            Nombres = "Test",
            Apellidos = "Cliente",
            FechaCheckoutPrevista = DateTime.UtcNow.AddDays(2)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CheckInAsync(dto, 1));
    }

    [Fact]
    public async Task CheckIn_ClienteAnonimo_CreaEstanciaConClienteAnonimo()
    {
        var db = CreateContext();
        var luaMock = CreateMockLuaService();
        var txManager = TestTransactionFactory.Create();
        var validador = new ValidadorEstadoService(db);
        var service = new EstanciaService(db, luaMock, txManager, validador, CreateMockHubContext());

        var dto = new CheckInDto
        {
            IdHabitacion = 1,
            UsarClienteAnonimo = true,
            FechaCheckoutPrevista = DateTime.UtcNow.AddDays(2),
            MetodoPago = "005"
        };

        var result = await service.CheckInAsync(dto, 1);
        Assert.NotNull(result);
        Assert.Contains("CLIENTE ANONIMO", result.ClienteNombreCompleto);
    }

    [Fact]
    public async Task CheckIn_ClienteNuevo_SeCreaAutomaticamente()
    {
        var db = CreateContext();
        var luaMock = CreateMockLuaService();
        var txManager = TestTransactionFactory.Create();
        var validador = new ValidadorEstadoService(db);
        var service = new EstanciaService(db, luaMock, txManager, validador, CreateMockHubContext());

        var dto = new CheckInDto
        {
            IdHabitacion = 1,
            TipoDocumento = "1",
            Documento = "99999999",
            Nombres = "Nuevo",
            Apellidos = "Usuario",
            FechaCheckoutPrevista = DateTime.UtcNow.AddDays(1),
            MetodoPago = "005"
        };

        var result = await service.CheckInAsync(dto, 1);
        Assert.NotNull(result);
        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Documento == "99999999");
        Assert.NotNull(cliente);
    }

    [Fact]
    public async Task CheckOut_EstanciaActiva_FinalizaCorrectamente()
    {
        var db = CreateContext();
        var luaMock = CreateMockLuaService();
        var txManager = TestTransactionFactory.Create();
        var validador = new ValidadorEstadoService(db);
        var service = new EstanciaService(db, luaMock, txManager, validador, CreateMockHubContext());

        var estancia = await service.CheckInAsync(new CheckInDto
        {
            IdHabitacion = 1,
            TipoDocumento = "1",
            Documento = "61077298",
            Nombres = "Test",
            Apellidos = "Cliente",
            FechaCheckoutPrevista = DateTime.UtcNow.AddDays(2),
            MetodoPago = "005"
        }, 1);

        var result = await service.CheckOutAsync(estancia.IdEstancia, 1);
        Assert.Equal("Finalizada", result.Estado);
        Assert.NotNull(result.FechaCheckoutReal);
    }

    [Fact]
    public async Task CrearReserva_SinSolapamiento_CreaReserva()
    {
        var db = CreateContext();
        var luaMock = CreateMockLuaService();
        var txManager = TestTransactionFactory.Create();
        var validador = new ValidadorEstadoService(db);
        var service = new EstanciaService(db, luaMock, txManager, validador, CreateMockHubContext());

        var dto = new ReservaCreateDto
        {
            IdHabitacion = 1,
            TipoDocumento = "1",
            Documento = "61077298",
            Nombres = "Test",
            Apellidos = "Cliente",
            FechaEntradaPrevista = DateTime.UtcNow.AddDays(10),
            FechaSalidaPrevista = DateTime.UtcNow.AddDays(12),
            MetodoPago = "005"
        };

        var result = await service.CrearReservaAsync(dto, 1);
        Assert.NotNull(result);
        Assert.Equal("Confirmada", result.Estado);
    }

    [Fact]
    public async Task CrearReserva_ConSolapamiento_LanzaExcepcion()
    {
        var db = CreateContext();
        var luaMock = CreateMockLuaService();
        var txManager = TestTransactionFactory.Create();
        var validador = new ValidadorEstadoService(db);
        var service = new EstanciaService(db, luaMock, txManager, validador, CreateMockHubContext());

        var dto1 = new ReservaCreateDto
        {
            IdHabitacion = 1,
            TipoDocumento = "1",
            Documento = "61077298",
            Nombres = "Test",
            Apellidos = "Cliente",
            FechaEntradaPrevista = DateTime.UtcNow.AddDays(10),
            FechaSalidaPrevista = DateTime.UtcNow.AddDays(15),
            MetodoPago = "005"
        };
        await service.CrearReservaAsync(dto1, 1);

        var dto2 = new ReservaCreateDto
        {
            IdHabitacion = 1,
            TipoDocumento = "1",
            Documento = "61077298",
            Nombres = "Test2",
            Apellidos = "Cliente2",
            FechaEntradaPrevista = DateTime.UtcNow.AddDays(12),
            FechaSalidaPrevista = DateTime.UtcNow.AddDays(17),
            MetodoPago = "005"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearReservaAsync(dto2, 1));
        Assert.Contains("ya está reservada", ex.Message);
    }

    [Fact]
    public async Task Transicion_Disponible_Ocupada_EsValida()
    {
        var db = TestDbContextFactory.Create();
        var validador = new ValidadorEstadoService(db);
        Assert.True(await validador.EsTransicionValidaAsync(1, 2));
    }

    [Fact]
    public async Task Transicion_Mantenimiento_Ocupada_NoPermitida()
    {
        var db = TestDbContextFactory.Create();
        var validador = new ValidadorEstadoService(db);
        Assert.False(await validador.EsTransicionValidaAsync(4, 2));
    }
}
