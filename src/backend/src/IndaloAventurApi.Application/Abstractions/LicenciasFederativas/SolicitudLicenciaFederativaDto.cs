namespace IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

public sealed record SolicitudLicenciaFederativaDto(
    Guid Id,
    int Temporada,
    string Estado,
    DateTime FechaCreacionUtc,
    int TarifaLicenciaFederativaId,
    string Licencia,
    string Categoria,
    string Territorio,
    bool MediaTemporada,
    decimal PrecioClub,
    decimal? PrecioIndependiente);
