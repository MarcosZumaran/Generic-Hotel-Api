using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Data;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelGenericoApi.Hubs;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<HotelDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention()    
);

// NLua
builder.Services.AddSingleton<ILuaService, LuaService>();

// Mappers
builder.Services.AddSingleton<EstadoHabitacionMapper>();
builder.Services.AddSingleton<RolUsuarioMapper>();
builder.Services.AddSingleton<MetodoPagoMapper>();
builder.Services.AddSingleton<TipoDocumentoMapper>();
builder.Services.AddSingleton<TipoComprobanteMapper>();
builder.Services.AddSingleton<AfectacionIgvMapper>();
builder.Services.AddSingleton<EstadoSunatMapper>();
builder.Services.AddSingleton<TiposHabitacionMapper>();
builder.Services.AddSingleton<UsuarioMapper>();
builder.Services.AddSingleton<ClienteMapper>();
builder.Services.AddSingleton<HabitacionMapper>();
builder.Services.AddSingleton<ProductoMapper>();

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
builder.Services.AddScoped<IEstanciaService, EstanciaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IComprobanteService, ComprobanteService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<ICierreCajaEnvioService, CierreCajaEnvioService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IConfiguracionHotelService, ConfiguracionHotelService>();
builder.Services.AddScoped<IValidadorEstadoService, ValidadorEstadoService>();

// Setup
builder.Services.AddScoped<SetupService>();

// Transacciones
builder.Services.AddScoped<IDbTransactionManager, SqlServerTransactionManager>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

// JWT
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// La ruta debe coincidir con el frontend: /hotelhub
app.MapHub<HabitacionHub>("/hotelhub");
app.MapControllers();

app.Run();