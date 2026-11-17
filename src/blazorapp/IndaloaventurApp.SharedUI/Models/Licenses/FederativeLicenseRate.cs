namespace IndaloaventurApp.SharedUI.Models.Licenses;

/// <summary>A catalog rate; the club price applies to the selected coverage without a daily multiplier.</summary>
/// <param name="Id">Rate identifier resolved by the full selection.</param>
/// <param name="Temporada">Season year.</param>
/// <param name="TipoLicenciaId">Catalog type identifier.</param>
/// <param name="TipoLicenciaNombre">Type name supplied by the API.</param>
/// <param name="Licencia">License name.</param>
/// <param name="Categoria">Category.</param>
/// <param name="Territorio">Territorial scope.</param>
/// <param name="PrecioClub">Club price.</param>
/// <param name="PrecioIndependiente">Separate independent price, when available.</param>
public sealed record FederativeLicenseRate(
    int Id,
    int Temporada,
    int TipoLicenciaId,
    string TipoLicenciaNombre,
    string Licencia,
    string Categoria,
    string Territorio,
    decimal PrecioClub,
    decimal? PrecioIndependiente);
