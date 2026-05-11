using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HotelGenericoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VentaController : ControllerBase
{
    private readonly IVentaService _service;

    public VentaController(IVentaService service)
    {
        _service = service;
    }

    private int? ObtenerIdUsuario()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null) return null;
        return int.TryParse(claim.Value, out int id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(VentaCreateDto dto)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            var result = await _service.CreateAsync(dto, idUsuario);
            return CreatedAtAction(nameof(GetById), new { id = result.IdVenta }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}