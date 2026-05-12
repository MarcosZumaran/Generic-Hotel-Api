using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HotelGenericoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstanciaController : ControllerBase
{
    private readonly IEstanciaService _service;

    public EstanciaController(IEstanciaService service)
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

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn(CheckInDto dto)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            var result = await _service.CheckInAsync(dto, idUsuario);
            return CreatedAtAction(nameof(GetById), new { id = result.IdEstancia }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{id}/checkout")]
    public async Task<IActionResult> CheckOut(int id)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            var result = await _service.CheckOutAsync(id, idUsuario);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
    [HttpPost("{id}/consumo")]
    public async Task<IActionResult> RegistrarConsumo(int id, ConsumoEstanciaCreateDto dto)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            var result = await _service.RegistrarConsumoAsync(id, dto, idUsuario);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("reserva")]
    public async Task<IActionResult> CrearReserva(ReservaCreateDto dto)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            var result = await _service.CrearReservaAsync(dto, idUsuario);
            return CreatedAtAction(nameof(GetReservaById), new { id = result.IdReserva }, result);
        }
        catch (InvalidOperationException ex)
        {
            // Si es un conflicto de solapamiento, devolver 409 Conflict
            if (ex.Message.Contains("ya está reservada") || ex.Message.Contains("solap"))
                return Conflict(new { mensaje = ex.Message });
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("reserva/{id}")]
    public async Task<IActionResult> GetReservaById(int id)
    {
        try
        {
            var result = await _service.GetReservaByIdAsync(id);
            return result is not null ? Ok(result) : NotFound(new { mensaje = "Reserva no encontrada." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("reservas/{idHabitacion}")]
    public async Task<IActionResult> GetReservasPorHabitacion(int idHabitacion)
    {
        try
        {
            var result = await _service.GetReservasPorHabitacionAsync(idHabitacion);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{id}/consumos")]
    public async Task<IActionResult> GetConsumos(int id)
    {
        try
        {
            var result = await _service.GetConsumosAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{idEstancia}/consumo/{idItem}")]
    public async Task<IActionResult> ActualizarConsumo(int idEstancia, int idItem, [FromBody] ActualizarConsumoDto dto)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            await _service.ActualizarConsumoAsync(idEstancia, idItem, dto.Cantidad, idUsuario);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{idEstancia}/consumo/{idItem}")]
    public async Task<IActionResult> EliminarConsumo(int idEstancia, int idItem)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            await _service.EliminarConsumoAsync(idEstancia, idItem, idUsuario);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("reserva/{id}/cancelar")]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();
            await _service.CancelarReservaAsync(id, idUsuario);
            return Ok(new { mensaje = "Reserva cancelada correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}