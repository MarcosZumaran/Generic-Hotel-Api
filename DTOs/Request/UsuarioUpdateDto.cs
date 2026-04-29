namespace LaRicaNoche.Api.DTOs.Request;

public sealed record UsuarioUpdateDto
{
    public string? Username { get; init; }
    public string? Password { get; init; }
    public int? IdRol { get; init; }
    public bool? EstaActivo { get; init; }
}