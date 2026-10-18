using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IndaloAventurApi.IntegrationTests;

public sealed class InventarioPersistenceMappingTests
{
    [Fact]
    public void Inventario_ModeloConfiguraClavesRestriccionesEIndices()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=indalo-mapping-tests;Trusted_Connection=True")
            .Options;

        using var dbContext = new ApplicationDbContext(options);
        var categoria = dbContext.Model.FindEntityType("IndaloAventurApi.Domain.Inventario.InventarioCategoria");
        var estado = dbContext.Model.FindEntityType("IndaloAventurApi.Domain.Inventario.InventarioEstado");

        Assert.NotNull(categoria);
        Assert.NotNull(estado);
        Assert.Equal("InventarioCategorias", categoria.GetTableName());
        Assert.Equal("InventarioEstados", estado.GetTableName());
        Assert.Equal(ValueGenerated.OnAdd, categoria.FindProperty("Id")!.ValueGenerated);
        Assert.Equal(ValueGenerated.OnAdd, estado.FindProperty("Id")!.ValueGenerated);
        Assert.False(categoria.FindProperty("Nombre")!.IsNullable);
        Assert.False(estado.FindProperty("Nombre")!.IsNullable);
        Assert.Equal(120, categoria.FindProperty("Nombre")!.GetMaxLength());
        Assert.Equal(120, estado.FindProperty("Nombre")!.GetMaxLength());

        var parentForeignKey = Assert.Single(categoria.GetForeignKeys());
        Assert.Equal(DeleteBehavior.Restrict, parentForeignKey.DeleteBehavior);
        Assert.True(parentForeignKey.IsRequired == false);

        Assert.Contains(categoria.GetIndexes(), index =>
            index.IsUnique
            && index.Properties.Select(property => property.Name).SequenceEqual(["CategoriaPadreId", "NombreNormalizado"])
            && index.GetFilter() == "[CategoriaPadreId] IS NOT NULL");
        Assert.Contains(categoria.GetIndexes(), index =>
            index.IsUnique
            && index.Properties.Select(property => property.Name).SequenceEqual(["NombreNormalizado"])
            && index.GetFilter() == "[CategoriaPadreId] IS NULL");
        Assert.Contains(estado.GetIndexes(), index =>
            index.IsUnique
            && index.Properties.Select(property => property.Name).SequenceEqual(["NombreNormalizado"]));
    }
}
