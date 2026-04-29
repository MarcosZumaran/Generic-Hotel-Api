namespace LaRicaNoche.Api.DTOs.Request;

public sealed record UsuarioCreateDto
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public int IdRol { get; init; }
}