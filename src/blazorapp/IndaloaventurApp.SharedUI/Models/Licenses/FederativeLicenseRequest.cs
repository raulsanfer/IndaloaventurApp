namespace IndaloaventurApp.SharedUI.Models.Licenses;

public sealed record FederativeLicenseRequest(
    Guid Id,
    int Temporada,
    bool MediaTemporada,
    string Licencia,
    string Categoria,
    string AmbitoTerritorial,
    string Estado);
