using Microsoft.AspNetCore.Mvc;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.Services.Implementations;
using Microsoft.AspNetCore.Authorization;

namespace HotelGenericoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SetupController : ControllerBase
{
    private readonly SetupService _setupService;

    public SetupController(SetupService setupService)
    {
        _setupService = setupService;
    }

    [HttpGet("estado")]
    public async Task<IActionResult> Estado()
    {
        var esPrimerInicio = await _setupService.EsPrimerInicioAsync();
        return Ok(new { requiereInicializacion = esPrimerInicio });
    }

    [HttpPost("crear-admin")]
    public async Task<IActionResult> CrearAdmin(UsuarioCreateDto dto)
    {
        try
        {
            var esPrimerInicio = await _setupService.EsPrimerInicioAsync();
            if (!esPrimerInicio)
                return BadRequest(new { mensaje = "El sistema ya fue inicializado. Use el endpoint de creación de usuarios autenticado." });

            await _setupService.CrearUsuarioAdminAsync(dto);
            return Ok(new { mensaje = "Usuario administrador creado exitosamente. El sistema está listo para usar." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("crear-usuarios-defecto")]
    public async Task<IActionResult> CrearUsuariosDefecto()
    {
        try
        {
            var esPrimerInicio = await _setupService.EsPrimerInicioAsync();
            if (!esPrimerInicio)
                return BadRequest(new { mensaje = "El sistema ya tiene usuarios. Este endpoint solo debe usarse en primer inicio o desarrollo." });

            await _setupService.CrearUsuariosPorDefectoAsync();
            return Ok(new { mensaje = "Usuarios por defecto creados: admin, recepcion, limpieza." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}