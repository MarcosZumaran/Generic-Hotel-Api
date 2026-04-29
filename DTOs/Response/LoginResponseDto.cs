namespace LaRicaNoche.Api.DTOs.Response;

public sealed record LoginResponseDto(
    string Token,
    DateTime Expiration,
    UsuarioResponseDto Usuario
);