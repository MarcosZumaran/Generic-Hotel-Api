using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelGenericoApi.Data;
using HotelGenericoApi.Models;
using Microsoft.Extensions.Logging;

namespace HotelGenericoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _service;
    private readonly HotelDbContext _db;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(IUsuarioService service, HotelDbContext db, ILogger<UsuarioController> logger)
    {
        _service = service;
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UsuarioCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdUsuario }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UsuarioUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = Request.Headers.UserAgent.ToString();

        try
        {
            var result = await _service.LoginAsync(dto);

            if (result is not null)
            {
                await RegistrarLoginAttempt(ipAddress, dto.Username, true, userAgent);
                _logger.LogInformation("Login exitoso para usuario {Username} desde IP {IpAddress}", dto.Username, ipAddress);
                return Ok(result);
            }
            else
            {
                await RegistrarLoginAttempt(ipAddress, dto.Username, false, userAgent);
                _logger.LogWarning("Login fallido para usuario {Username} desde IP {IpAddress}", dto.Username, ipAddress);
                return Unauthorized();
            }
        }
        catch (Exception ex)
        {
            await RegistrarLoginAttempt(ipAddress, dto.Username, false, userAgent);
            _logger.LogWarning(ex, "Login fallido para usuario {Username} desde IP {IpAddress}", dto.Username, ipAddress);
            throw;
        }
    }

    private async Task RegistrarLoginAttempt(string ipAddress, string? username, bool succeeded, string? userAgent)
    {
        try
        {
            var attempt = new LoginAttempt
            {
                IpAddress = ipAddress,
                Username = username,
                AttemptedAt = DateTime.UtcNow,
                Succeeded = succeeded,
                UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent
            };
            _db.LoginAttempts.Add(attempt);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar LoginAttempt");
        }
    }
}