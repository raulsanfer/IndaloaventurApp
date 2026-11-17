namespace IndaloaventurApp.SharedUI.Models.Licenses;

/// <summary>Creates a request for the authenticated member; identity and price are resolved by the API.</summary>
/// <param name="Temporada">Season year.</param>
/// <param name="TarifaLicenciaFederativaId">Selected catalog rate.</param>
/// <param name="FechaInicio">Date-only coverage start, required for Día.</param>
/// <param name="FechaFin">Date-only coverage end, required for Día and allowing the same date as start.</param>
public sealed record CreateFederativeLicenseRequest(
    int Temporada,
    int TarifaLicenciaFederativaId,
    DateOnly? FechaInicio = null,
    DateOnly? FechaFin = null);
