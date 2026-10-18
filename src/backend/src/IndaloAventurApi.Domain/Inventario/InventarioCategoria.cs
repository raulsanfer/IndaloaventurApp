using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.Inventario;

public sealed class InventarioCategoria : Entity, IAggregateRoot
{
    private InventarioCategoria()
    {
    }

    private InventarioCategoria(InventarioNombre nombre, int? categoriaPadreId)
    {
        AplicarNombre(nombre);
        CategoriaPadreId = categoriaPadreId;
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string NombreNormalizado { get; private set; } = string.Empty;
    public int? CategoriaPadreId { get; private set; }

    public static InventarioCategoria Crear(string nombre, int? categoriaPadreId)
        => new(InventarioNombre.Crear(nombre), categoriaPadreId);

    public void Actualizar(string nombre, int? categoriaPadreId)
    {
        AplicarNombre(InventarioNombre.Crear(nombre));
        CategoriaPadreId = categoriaPadreId;
    }

    private void AplicarNombre(InventarioNombre nombre)
    {
        Nombre = nombre.Valor;
        NombreNormalizado = nombre.ValorNormalizado;
    }
}
