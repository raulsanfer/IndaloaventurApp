using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.ClubPositions;

public sealed class Cargo : Entity, IAggregateRoot
{
    private Cargo()
    {
    }

    private Cargo(string descripcion)
    {
        Descripcion = descripcion;
    }

    public int Id { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;

    public static Cargo Crear(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion)) throw new DomainException("La descripcion del cargo es obligatoria.");
        return new Cargo(descripcion.Trim());
    }

    public void Actualizar(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion)) throw new DomainException("La descripcion del cargo es obligatoria.");
        Descripcion = descripcion.Trim();
    }
}
