using Microsoft.AspNetCore.Mvc;
using LaRicaNoche.Api.Services.Interfaces;

namespace LaRicaNoche.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReporteController : ControllerBase
{
    private readonly IReporteService _service;
    private readonly ICierreCajaEnvioService _cierreCajaEnvioService;


    public ReporteController(IReporteService service, ICierreCajaEnvioService cierreCajaEnvioService)
    {
        _service = service;
        _cierreCajaEnvioService = cierreCajaEnvioService;
    }

    [HttpGet("cierre-caja")]
    public async Task<IActionResult> CierreCaja([FromQuery] DateOnly? fecha)
        => Ok(await _service.GetCierreCajaAsync(fecha));

    [HttpGet("estado-habitaciones")]
    public async Task<IActionResult> EstadoHabitaciones()
        => Ok(await _service.GetEstadoHabitacionesAsync());

    [HttpGet("cierre-caja/estado-envio")]
    public async Task<IActionResult> EstadoEnvioCierreCaja([FromQuery] DateOnly fecha)
    {
        var estado = await _cierreCajaEnvioService.GetEstadoAsync(fecha);
        return Ok(estado);
    }

    [HttpPost("cierre-caja/enviar")]
    public async Task<IActionResult> EnviarCierreCaja([FromQuery] DateOnly fecha)
    {
        var result = await _cierreCajaEnvioService.MarcarComoEnviadoAsync(fecha);
        return result ? NoContent() : BadRequest();
    }
}