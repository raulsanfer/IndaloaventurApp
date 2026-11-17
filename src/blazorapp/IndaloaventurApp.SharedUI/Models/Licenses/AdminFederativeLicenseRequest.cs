namespace IndaloaventurApp.SharedUI.Models.Licenses;

/// <summary>Administrative request data. State changes preserve type, rate, owner, creation and coverage.</summary>
/// <param name="Id">Unique request identifier.</param>
/// <param name="UserId">Request owner.</param>
/// <param name="UserEmail">Owner display email.</param>
/// <param name="Temporada">Season year.</param>
/// <param name="Estado">Editable administrative state.</param>
/// <param name="FechaCreacionUtc">Original UTC creation timestamp.</param>
/// <param name="TarifaLicenciaFederativaId">Original selected rate.</param>
/// <param name="Licencia">License name.</param>
/// <param name="Categoria">Category.</param>
/// <param name="AmbitoTerritorial">Territorial scope.</param>
/// <param name="PrecioClub">Club price.</param>
/// <param name="PrecioIndependiente">Separate independent price.</param>
/// <param name="TipoLicenciaId">Catalog type identifier.</param>
/// <param name="TipoLicenciaNombre">Type name supplied by the API.</param>
/// <param name="FechaInicio">Optional date-only coverage start.</param>
/// <param name="FechaFin">Optional date-only coverage end.</param>
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
    decimal? PrecioIndependiente,
    int TipoLicenciaId,
    string TipoLicenciaNombre,
    DateOnly? FechaInicio = null,
    DateOnly? FechaFin = null);
