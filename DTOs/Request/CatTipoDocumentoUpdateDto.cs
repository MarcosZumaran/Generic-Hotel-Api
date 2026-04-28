namespace LaRicaNoche.Api.DTOs.Request;

public sealed record CatTipoDocumentoUpdateDto
{
    public string? Descripcion { get; init; }
}