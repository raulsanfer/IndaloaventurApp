namespace IndaloaventurApp.SharedUI.Models.Licenses;

public sealed record AdminFederativeLicenseRequest(
    Guid Id,
    Guid UserId,
    string UserEmail,
    int Temporada,
    string Estado,
    DateTime FechaCreacionUtc,
    int TarifaLicenciaFederativaId,
    string Licencia,
    string Categoria,
    string AmbitoTerritorial,
    decimal PrecioClub,
    decimal? PrecioIndependiente);
