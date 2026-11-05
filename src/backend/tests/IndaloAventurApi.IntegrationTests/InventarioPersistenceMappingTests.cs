using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
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

    [Fact]
    public void ReservasMaterial_ModeloSqlServerConfiguraIntegridadIndicesYConcurrencia()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=indalo-reservas-mapping-tests;Trusted_Connection=True")
            .Options;

        using var dbContext = new ApplicationDbContext(options);
        var reserva = dbContext.Model.FindEntityType(typeof(ReservaMaterial));

        Assert.NotNull(reserva);
        Assert.Equal("ReservasMaterial", reserva!.GetTableName());
        Assert.False(reserva.FindProperty(nameof(ReservaMaterial.UserId))!.IsNullable);
        Assert.False(reserva.FindProperty(nameof(ReservaMaterial.InventarioMaterialId))!.IsNullable);
        Assert.False(reserva.FindProperty(nameof(ReservaMaterial.CantidadSolicitada))!.IsNullable);
        Assert.False(reserva.FindProperty(nameof(ReservaMaterial.FechaSolicitudUtc))!.IsNullable);
        Assert.True(reserva.FindProperty(nameof(ReservaMaterial.ConcurrencyToken))!.IsConcurrencyToken);
        Assert.Contains(reserva.GetIndexes(), index =>
            index.Properties.Select(property => property.Name).SequenceEqual(["InventarioMaterialId", "Estado"]));
        Assert.Contains(reserva.GetIndexes(), index =>
            index.Properties.Select(property => property.Name).SequenceEqual(["UserId", "FechaSolicitudUtc"]));
        Assert.All(reserva.GetForeignKeys(), foreignKey => Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
    }
}
