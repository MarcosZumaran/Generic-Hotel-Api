using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using LaRicaNoche.Api.Mappings;
using LaRicaNoche.Api.Data;
using LaRicaNoche.Api.Services.Interfaces;
using LaRicaNoche.Api.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddSingleton<CatTipoComprobanteMapper>();
builder.Services.AddSingleton<CatAfectacionIgvMapper>();
builder.Services.AddSingleton<CatEstadoSunatMapper>();
builder.Services.AddSingleton<TiposHabitacionMapper>();
builder.Services.AddSingleton<UsuarioMapper>();
builder.Services.AddSingleton<ClienteMapper>();
builder.Services.AddSingleton<HabitacionMapper>();

// Servicios
builder.Services.AddScoped<ICatEstadoHabitacionService, CatEstadoHabitacionService>();
builder.Services.AddScoped<ICatRolUsuarioService, CatRolUsuarioService>();
builder.Services.AddScoped<ICatMetodoPagoService, CatMetodoPagoService>();
builder.Services.AddScoped<ICatTipoDocumentoService, CatTipoDocumentoService>();
builder.Services.AddScoped<ICatTipoComprobanteService, CatTipoComprobanteService>();
builder.Services.AddScoped<ICatAfectacionIgvService, CatAfectacionIgvService>();
builder.Services.AddScoped<ICatEstadoSunatService, CatEstadoSunatService>();
builder.Services.AddScoped<ITiposHabitacionService, TiposHabitacionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IHabitacionService, HabitacionService>();
// Configuración JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

//  Controladores y OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // /scalar/v1
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();