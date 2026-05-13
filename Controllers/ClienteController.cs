using Microsoft.AspNetCore.Mvc;
using HotelGenericoApi.DTOs.Request;
using HotelGenericoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelGenericoApi.DTOs.Response;

namespace HotelGenericoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _service;
    private readonly IConfiguration _configuration;

    public ClienteController(IClienteService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
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

    [HttpGet("documento/{tipo}/{documento}")]
    public async Task<IActionResult> GetByDocumento(string tipo, string documento)
    {
        var result = await _service.GetByDocumentoAsync(tipo, documento);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ClienteCreateDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.IdCliente }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ClienteUpdateDto dto)
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


    // Endpoint para consultar datos de RENIEC mediante VerificaPE
    [HttpGet("reniec/{dni}")]
    public async Task<IActionResult> ConsultarReniec(string dni)
    {
        var apiKey = _configuration["VerificaPE:ApiKey"];
        var baseUrl = _configuration["VerificaPE:BaseUrl"];

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var response = await httpClient.GetAsync($"{baseUrl}/dni/{dni}");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpGet("buscar")]
    [Authorize]
    public async Task<IActionResult> BuscarClientes([FromQuery] string termino)
    {
        if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
            return Ok(Array.Empty<ClienteResponseDto>());

        var resultados = await _service.BuscarAsync(termino, 5);
        return Ok(resultados);
    }
}