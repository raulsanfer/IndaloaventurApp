using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.LicenciasFederativas;

public sealed class TarifaLicenciaFederativa : Entity, IAggregateRoot
{
    private TarifaLicenciaFederativa()
    {
    }

    private TarifaLicenciaFederativa(
        int id,
        int temporada,
        string licencia,
        string categoria,
        decimal precioClub,
        decimal? precioIndependiente,
        string territorio,
        int tipoLicenciaId)
    {
        Id = id;
        Temporada = temporada;
        Licencia = licencia;
        Categoria = categoria;
        PrecioClub = precioClub;
        PrecioIndependiente = precioIndependiente;
        Territorio = territorio;
        TipoLicenciaId = tipoLicenciaId;
    }

    public int Id { get; private set; }
    public int Temporada { get; private set; }
    public string Licencia { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public decimal PrecioClub { get; private set; }
    public decimal? PrecioIndependiente { get; private set; }
    public string Territorio { get; private set; } = string.Empty;
    public int TipoLicenciaId { get; private set; }
    public TipoLicencia TipoLicencia { get; private set; } = null!;

    public static TarifaLicenciaFederativa Crear(
        int temporada,
        string licencia,
        string categoria,
        decimal precioClub,
        decimal? precioIndependiente,
        string territorio,
        int tipoLicenciaId)
    {
        Validar(temporada, licencia, categoria, precioClub, precioIndependiente, territorio);
        if (tipoLicenciaId <= 0)
        {
            throw new DomainException("El tipo de licencia de la tarifa es obligatorio.");
        }

        return new TarifaLicenciaFederativa(
            0,
            temporada,
            licencia.Trim(),
            categoria.Trim(),
            precioClub,
            precioIndependiente,
            territorio.Trim(),
            tipoLicenciaId);
    }

    private static void Validar(
        int temporada,
        string licencia,
        string categoria,
        decimal precioClub,
        decimal? precioIndependiente,
        string territorio)
    {
        if (temporada <= 0)
        {
            throw new DomainException("La temporada de la tarifa es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(licencia))
        {
            throw new DomainException("La licencia de la tarifa es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(categoria))
        {
            throw new DomainException("La categoria de la tarifa es obligatoria.");
        }

        if (precioClub <= 0)
        {
            throw new DomainException("El precio para clubes de la tarifa debe ser mayor que 0.");
        }

        if (precioIndependiente.HasValue && precioIndependiente.Value <= 0)
        {
            throw new DomainException("El precio para independientes de la tarifa debe ser mayor que 0.");
        }

        if (string.IsNullOrWhiteSpace(territorio))
        {
            throw new DomainException("El territorio de la tarifa es obligatorio.");
        }
    }
}
