using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LuaTestController : ControllerBase
{
    private readonly ILuaService _lua;

    public LuaTestController(ILuaService lua) => _lua = lua;

    [HttpGet("validar-cliente")]
    public IActionResult ValidarCliente(string documento, string tipo)
    {
        var result = _lua.CallFunction("validar_cliente.lua", "Validar", documento, tipo);
        bool valido = result.Length > 0 && Convert.ToBoolean(result[0]);
        return Ok(new { documento, tipo, valido });
    }
}