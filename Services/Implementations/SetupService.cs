using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.Models;
using HotelGenericoApi.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HotelGenericoApi.Services.Implementations;

public class SetupService
{
    private readonly HotelDbContext _db;

    public SetupService(HotelDbContext db)
    {
        _db = db;
    }

    public async Task<bool> EsPrimerInicioAsync()
    {
        return !await _db.Usuarios.AnyAsync();
    }

    public async Task CrearUsuarioAdminAsync(UsuarioCreateDto dto)
    {
        if (!await EsPrimerInicioAsync())
            throw new BusinessRuleViolationException(BusinessErrorCode.SetupAlreadyDone, "El sistema ya fue inicializado.");

        var rolAdmin = await _db.RolesUsuario.FirstOrDefaultAsync(r => r.Nombre == "Administrador")
            ?? throw new BusinessRuleViolationException(BusinessErrorCode.ValidationError, "Rol Administrador no encontrado en el catálogo.");

        var usuario = new Usuario
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IdRol = rolAdmin.IdRol,
            FechaCreacion = DateTime.UtcNow,
            EstaActivo = true
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
    }

    /// Crea los usuarios por defecto (admin, recepcion, limpieza) si no existen.
    /// Para desarrollo y  primera instalación.
    public async Task CrearUsuariosPorDefectoAsync()
    {
        // 1. Asegurar que los roles existen
        var roles = new[] { "Administrador", "Recepcion", "Limpieza" };
        foreach (var nombreRol in roles)
        {
            if (!await _db.RolesUsuario.AnyAsync(r => r.Nombre == nombreRol))
            {
                _db.RolesUsuario.Add(new RolUsuario { Nombre = nombreRol });
            }
        }
        await _db.SaveChangesAsync();

        // 2. Obtener IDs de los roles
        var rolAdmin = await _db.RolesUsuario.FirstAsync(r => r.Nombre == "Administrador");
        var rolRecepcion = await _db.RolesUsuario.FirstAsync(r => r.Nombre == "Recepcion");
        var rolLimpieza = await _db.RolesUsuario.FirstAsync(r => r.Nombre == "Limpieza");

        // 3. Crear usuarios por defecto si no existen
        var usuariosPorDefecto = new (string Username, string Password, int IdRol)[]
        {
            ("admin", "admin123", rolAdmin.IdRol),
            ("recepcion", "recepcion123", rolRecepcion.IdRol),
            ("limpieza", "limpieza123", rolLimpieza.IdRol)
        };

        foreach (var (username, password, idRol) in usuariosPorDefecto)
        {
            if (!await _db.Usuarios.AnyAsync(u => u.Username == username))
            {
                _db.Usuarios.Add(new Usuario
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    IdRol = idRol,
                    FechaCreacion = DateTime.UtcNow,
                    EstaActivo = true
                });
            }
        }

        await _db.SaveChangesAsync();
    }
}