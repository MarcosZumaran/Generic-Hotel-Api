using Microsoft.AspNetCore.Mvc;
using LaRicaNoche.Api.DTOs.Request;
using LaRicaNoche.Api.Services.Interfaces;

namespace LaRicaNoche.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatTipoComprobanteController : ControllerBase
{
    private readonly ICatTipoComprobanteService _service;

    public CatTipoComprobanteController(ICatTipoComprobanteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{codigo}")]
    public async Task<IActionResult> GetById(string codigo)
    {
        var result = await _service.GetByIdAsync(codigo);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CatTipoComprobanteCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { codigo = result.Codigo }, result);
    }

    [HttpPut("{codigo}")]
    public async Task<IActionResult> Update(string codigo, CatTipoComprobanteUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(codigo, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{codigo}")]
    public async Task<IActionResult> Delete(string codigo)
    {
        var deleted = await _service.DeleteAsync(codigo);
        return deleted ? NoContent() : NotFound();
    }
}