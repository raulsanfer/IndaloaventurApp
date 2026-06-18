namespace IndaloaventurApp.SharedUI.Models.Licenses;

public sealed record FederativeLicenseRate(
    int Id,
    int Temporada,
    bool MediaTemporada,
    string Licencia,
    string Categoria,
    string Territorio,
    decimal PrecioClub,
    decimal? PrecioIndependiente);
