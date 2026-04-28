using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using LaRicaNoche.Api.Mappings;
using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.Services.Interfaces;
using LaRicaNoche.Api.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<LaRicaNocheDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// NLua
builder.Services.AddSingleton<ILuaService, LuaService>();

// Mappers
builder.Services.AddSingleton<CatEstadoHabitacionMapper>();
builder.Services.AddSingleton<CatRolUsuarioMapper>();
builder.Services.AddSingleton<CatMetodoPagoMapper>();
builder.Services.AddSingleton<CatTipoDocumentoMapper>();

// Servicios
builder.Services.AddScoped<ICatEstadoHabitacionService, CatEstadoHabitacionService>();
builder.Services.AddScoped<ICatRolUsuarioService, CatRolUsuarioService>();
builder.Services.AddScoped<ICatMetodoPagoService, CatMetodoPagoService>();
builder.Services.AddScoped<ICatTipoDocumentoService, CatTipoDocumentoService>();

//  Controladores y OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // /scalar/v1
}

app.MapControllers();

app.Run();