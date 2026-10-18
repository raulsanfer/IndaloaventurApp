using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.Inventario;

namespace IndaloAventurApi.Domain.Tests;

public sealed class InventarioTests
{
    [Fact]
    public void InventarioNombre_Crear_NormalizaYRechazaEntradasInvalidas()
    {
        var nombre = InventarioNombre.Crear("  Material tecnico  ");

        Assert.Equal("Material tecnico", nombre.Valor);
        Assert.Equal("MATERIAL TECNICO", nombre.ValorNormalizado);
        Assert.Throws<DomainException>(() => InventarioNombre.Crear(" "));
        Assert.Throws<DomainException>(() => InventarioNombre.Crear("Material\u0001"));
        Assert.Throws<DomainException>(() => InventarioNombre.Crear(new string('a', InventarioNombre.MaxLength + 1)));
    }

    [Fact]
    public void InventarioCategoriaYEstado_Crear_ConservanNombreNormalizado()
    {
        var categoria = InventarioCategoria.Crear("  Cascos ", null);
        var estado = InventarioEstado.Crear(" Disponible ");

        Assert.Equal(0, categoria.Id);
        Assert.Equal("Cascos", categoria.Nombre);
        Assert.Equal("CASCOS", categoria.NombreNormalizado);
        Assert.Null(categoria.CategoriaPadreId);
        Assert.Equal("Disponible", estado.Nombre);
        Assert.Equal("DISPONIBLE", estado.NombreNormalizado);
    }
}
