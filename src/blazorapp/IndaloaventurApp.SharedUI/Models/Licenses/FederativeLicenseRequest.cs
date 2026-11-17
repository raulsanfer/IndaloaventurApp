namespace IndaloaventurApp.SharedUI.Models.Licenses;

/// <summary>A member's request, identified independently of season, with optional date-only coverage.</summary>
/// <param name="Id">Unique request identifier.</param>
/// <param name="Temporada">Season year.</param>
/// <param name="TipoLicenciaId">Catalog type identifier.</param>
/// <param name="TipoLicenciaNombre">Type name supplied by the API.</param>
/// <param name="Licencia">License name.</param>
/// <param name="Categoria">Category.</param>
/// <param name="AmbitoTerritorial">Territorial scope.</param>
/// <param name="Estado">Current request status.</param>
/// <param name="FechaInicio">Coverage start; null for migrated requests without coverage.</param>
/// <param name="FechaFin">Coverage end; null for migrated requests without coverage.</param>
public sealed record FederativeLicenseRequest(
    Guid Id,
    int Temporada,
    int TipoLicenciaId,
    string TipoLicenciaNombre,
    string Licencia,
    string Categoria,
    string AmbitoTerritorial,
    string Estado,
    DateOnly? FechaInicio = null,
    DateOnly? FechaFin = null);
