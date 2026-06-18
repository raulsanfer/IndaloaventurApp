namespace IndaloaventurApp.SharedUI.Models.Licenses;

public sealed record FederativeLicenseSeasonGroup(
    int Temporada,
    IReadOnlyList<FederativeLicenseRequest> Solicitudes);
