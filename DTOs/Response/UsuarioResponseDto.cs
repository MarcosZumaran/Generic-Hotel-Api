namespace LaRicaNoche.Api.DTOs.Response;

public sealed record UsuarioResponseDto(
    int IdUsuario,
    string Username,
    int IdRol,
    string NombreRol,
    bool EstaActivo,
    DateTime FechaCreacion
);