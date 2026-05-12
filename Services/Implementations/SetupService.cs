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
}