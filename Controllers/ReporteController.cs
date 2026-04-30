using Microsoft.AspNetCore.Mvc;
using LaRicaNoche.Api.Services.Interfaces;

namespace LaRicaNoche.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReporteController : ControllerBase
{
    private readonly IReporteService _service;

    public ReporteController(IReporteService service) => _service = service;

    [HttpGet("cierre-caja")]
    public async Task<IActionResult> CierreCaja([FromQuery] DateOnly? fecha)
        => Ok(await _service.GetCierreCajaAsync(fecha));

    [HttpGet("estado-habitaciones")]
    public async Task<IActionResult> EstadoHabitaciones()
        => Ok(await _service.GetEstadoHabitacionesAsync());
}