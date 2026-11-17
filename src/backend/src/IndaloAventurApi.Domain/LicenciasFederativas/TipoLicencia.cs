using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.LicenciasFederativas;

public sealed class TipoLicencia : Entity, IAggregateRoot
{
    public const int CompletaId = 1;
    public const int MediaId = 2;
    public const int DiaId = 3;

    private TipoLicencia() { }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;

    public static TipoLicencia Crear(int id, string nombre)
    {
        if (id <= 0 || string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 80)
        {
            throw new DomainException("El tipo de licencia requiere un identificador positivo y un nombre de hasta 80 caracteres.");
        }

        return new TipoLicencia { Id = id, Nombre = nombre.Trim() };
    }
}
