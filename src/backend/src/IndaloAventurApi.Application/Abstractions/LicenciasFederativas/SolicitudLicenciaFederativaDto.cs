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
    int TipoLicenciaId,
    string TipoLicenciaNombre,
    decimal PrecioClub,
    decimal? PrecioIndependiente,
    DateOnly? FechaInicio,
    DateOnly? FechaFin);
