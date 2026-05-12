namespace HotelGenericoApi.DTOs.Response;

public sealed record ReservaResponseDto(
    int IdReserva,
    int IdHabitacion,
    string? NumeroHabitacion,
    string? ClienteNombre,
    DateTime FechaEntradaPrevista,
    DateTime FechaSalidaPrevista,
    decimal MontoTotal,
    string Estado,
    string? DocumentoCliente,
    string? Observaciones,
    bool EsNoShow
);