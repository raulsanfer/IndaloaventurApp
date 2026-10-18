using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.Inventario;

public sealed class InventarioEstado : Entity, IAggregateRoot
{
    private InventarioEstado()
    {
    }

    private InventarioEstado(InventarioNombre nombre)
    {
        AplicarNombre(nombre);
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string NombreNormalizado { get; private set; } = string.Empty;

    public static InventarioEstado Crear(string nombre)
        => new(InventarioNombre.Crear(nombre));

    public void Actualizar(string nombre)
        => AplicarNombre(InventarioNombre.Crear(nombre));

    private void AplicarNombre(InventarioNombre nombre)
    {
        Nombre = nombre.Valor;
        NombreNormalizado = nombre.ValorNormalizado;
    }
}
