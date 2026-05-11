using Microsoft.EntityFrameworkCore;
using HotelGenericoApi.Data;
using HotelGenericoApi.Models;

namespace HotelGenericoApi.Tests;

public static class TestDbContextFactory
{
    public static HotelDbContext Create()
    {
        var options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var db = new HotelDbContext(options);

        db.CatEstadoHabitacions.AddRange(
            new CatEstadoHabitacion { IdEstado = 1, Nombre = "Disponible", PermiteCheckin = true, PermiteCheckout = false, EsEstadoFinal = false },
            new CatEstadoHabitacion { IdEstado = 2, Nombre = "Ocupada", PermiteCheckin = false, PermiteCheckout = true, EsEstadoFinal = false },
            new CatEstadoHabitacion { IdEstado = 3, Nombre = "Limpieza", PermiteCheckin = false, PermiteCheckout = false, EsEstadoFinal = false },
            new CatEstadoHabitacion { IdEstado = 4, Nombre = "Mantenimiento", PermiteCheckin = false, PermiteCheckout = false, EsEstadoFinal = false }
        );

        db.CatTransicionEstados.AddRange(
            new CatTransicionEstado { IdEstadoActual = 1, IdEstadoSiguiente = 2 },
            new CatTransicionEstado { IdEstadoActual = 2, IdEstadoSiguiente = 3 },
            new CatTransicionEstado { IdEstadoActual = 3, IdEstadoSiguiente = 1 },
            new CatTransicionEstado { IdEstadoActual = 1, IdEstadoSiguiente = 4 },
            new CatTransicionEstado { IdEstadoActual = 4, IdEstadoSiguiente = 1 }
        );

        db.TiposHabitacions.Add(new TiposHabitacion { IdTipo = 1, Nombre = "Matrimonial", PrecioBase = 50m });

        db.Habitaciones.Add(new Habitacione
        {
            IdHabitacion = 1,
            NumeroHabitacion = "101",
            IdTipo = 1,
            PrecioNoche = 50m,
            IdEstado = 1,
            Piso = 1
        });

        db.Habitaciones.Add(new Habitacione
        {
            IdHabitacion = 2,
            NumeroHabitacion = "102",
            IdTipo = 1,
            PrecioNoche = 50m,
            IdEstado = 2,
            Piso = 1
        });

        db.Clientes.Add(new Cliente
        {
            IdCliente = 1,
            TipoDocumento = "0",
            Documento = "00000000",
            Nombres = "CLIENTE",
            Apellidos = "ANONIMO"
        });

        db.Clientes.Add(new Cliente
        {
            IdCliente = 2,
            TipoDocumento = "1",
            Documento = "61077298",
            Nombres = "Test",
            Apellidos = "Cliente"
        });

        db.ConfiguracionHotels.Add(new ConfiguracionHotel
        {
            Id = 1,
            Nombre = "Hotel Test",
            TasaIgvHotel = 10.5m
        });

        db.Tarifas.Add(new Tarifa
        {
            IdTarifa = 1,
            IdTipoHabitacion = 1,
            Precio = 50m
        });

        db.SaveChanges();
        return db;
    }
}
