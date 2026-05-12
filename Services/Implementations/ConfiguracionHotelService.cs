using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelGenericoApi.Services.Implementations;

public class ConfiguracionHotelService : IConfiguracionHotelService
{
    private readonly HotelDbContext _db;

    public ConfiguracionHotelService(HotelDbContext db)
    {
        _db = db;
    }

    public async Task<ConfiguracionHotelResponseDto> GetConfiguracionAsync()
    {
        var config = await _db.Configuracion.FirstOrDefaultAsync();

        return config is not null
            ? new ConfiguracionHotelResponseDto(
                config.Nombre,
                config.Direccion,
                config.Telefono,
                config.Ruc,
                config.TasaIgvHotel,
                config.TasaIgvProductos)
            : new ConfiguracionHotelResponseDto("Hotel Genérico", null, null, null, 10.5m, 18.0m);
    }
}