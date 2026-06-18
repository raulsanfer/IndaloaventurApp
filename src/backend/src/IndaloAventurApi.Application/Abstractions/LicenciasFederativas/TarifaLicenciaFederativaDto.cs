namespace IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

public sealed record TarifaLicenciaFederativaDto(
    int Id,
    int Temporada,
    string Licencia,
    string Categoria,
    string Territorio,
    bool MediaTemporada,
    decimal PrecioClub,
    decimal? PrecioIndependiente);
