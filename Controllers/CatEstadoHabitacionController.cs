using Microsoft.AspNetCore.Mvc;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.Services.Interfaces;

namespace LaRicaNoche.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatEstadoHabitacionController : ControllerBase
{
    private readonly ICatEstadoHabitacionService _service;

    public CatEstadoHabitacionController(ICatEstadoHabitacionService service)
    {
        _service = service;
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
    public async Task<IActionResult> Create(CatEstadoHabitacionCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdEstado }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CatEstadoHabitacionUpdateDto dto)
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
}