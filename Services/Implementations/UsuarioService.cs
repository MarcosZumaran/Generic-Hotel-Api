using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HotelGenericoApi.Data;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.DTOs.Response;
using HotelGenericoApi.Mappings;
using HotelGenericoApi.Services.Interfaces;
using HotelGenericoApi.Models;

namespace HotelGenericoApi.Services.Implementations;

public class UsuarioService : IUsuarioService
{
    private readonly HotelDbContext _db;
    private readonly UsuarioMapper _mapper;
    private readonly IConfiguration _configuration;

    public UsuarioService(HotelDbContext db, UsuarioMapper mapper, IConfiguration configuration)
    {
        _db = db;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<IEnumerable<UsuarioResponseDto>> GetAllAsync()
    {
        var usuarios = await _db.Usuarios
            .Include(u => u.IdRolNavigation)   // EF crea esta navegación
            .AsNoTracking()
            .ToListAsync();

        return usuarios.Select(u => new UsuarioResponseDto(
            u.IdUsuario,
            u.Username,
            u.IdRol ?? 0,
            u.IdRolNavigation?.Nombre ?? "",
            u.EstaActivo ?? false,
            u.FechaCreacion ?? DateTime.MinValue
        ));
    }

    public async Task<UsuarioResponseDto?> GetByIdAsync(int id)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.IdRolNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuario is null) return null;

        return new UsuarioResponseDto(
            usuario.IdUsuario,
            usuario.Username,
            usuario.IdRol ?? 0,
            usuario.IdRolNavigation?.Nombre ?? "",
            usuario.EstaActivo ?? false,
            usuario.FechaCreacion ?? DateTime.MinValue
        );
    }

    public async Task<UsuarioResponseDto> CreateAsync(UsuarioCreateDto dto)
    {
        var entity = _mapper.FromCreate(dto);
        entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        entity.FechaCreacion = DateTime.UtcNow;
        entity.EstaActivo = true;

        _db.Usuarios.Add(entity);
        await _db.SaveChangesAsync();

        // Recargar con navegación para obtener el nombre del rol
        await _db.Entry(entity).Reference(u => u.IdRolNavigation).LoadAsync();

        return new UsuarioResponseDto(
            entity.IdUsuario,
            entity.Username,
            entity.IdRol ?? 0,
            entity.IdRolNavigation?.Nombre ?? "",
            entity.EstaActivo ?? false,
            entity.FechaCreacion ?? DateTime.MinValue
        );
    }

    public async Task<bool> UpdateAsync(int id, UsuarioUpdateDto dto)
    {
        var entity = await _db.Usuarios.FindAsync(id);
        if (entity is null) return false;

        _mapper.UpdateFromDto(dto, entity);

        if (!string.IsNullOrEmpty(dto.Password))
        {
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Usuarios.FindAsync(id);
        if (entity is null) return false;

        _db.Usuarios.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.EstaActivo == true);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return null;

        var token = GenerarToken(usuario);
        var usuarioDto = new UsuarioResponseDto(
            usuario.IdUsuario,
            usuario.Username,
            usuario.IdRol ?? 0,
            usuario.IdRolNavigation?.Nombre ?? "",
            usuario.EstaActivo ?? false,
            usuario.FechaCreacion ?? DateTime.MinValue
        );

        return new LoginResponseDto(token, DateTime.UtcNow.AddHours(8), usuarioDto);
    }

    private string GenerarToken(Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Role, usuario.IdRolNavigation?.Nombre ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}