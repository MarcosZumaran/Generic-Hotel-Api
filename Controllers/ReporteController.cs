using Microsoft.AspNetCore.Mvc;
using HotelGenericoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HotelGenericoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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

    [HttpGet("cierre-caja/excel")]
    public async Task<IActionResult> ExportarCierreCajaExcel([FromQuery] DateOnly? fecha)
    {
        var bytes = await _service.ExportarCierreCajaExcelAsync(fecha);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"cierre_caja_{fecha:yyyyMMdd}.xlsx");
    }

    [HttpGet("estado-habitaciones/excel")]
    public async Task<IActionResult> ExportarEstadoHabitacionesExcel()
    {
        var bytes = await _service.ExportarEstadoHabitacionesExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "estado_habitaciones.xlsx");
    }
}