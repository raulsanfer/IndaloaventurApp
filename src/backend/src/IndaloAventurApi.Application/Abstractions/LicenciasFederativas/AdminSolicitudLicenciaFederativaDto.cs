namespace IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

public sealed record AdminSolicitudLicenciaFederativaDto(
    Guid Id,
    Guid UserId,
    string UserEmail,
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
