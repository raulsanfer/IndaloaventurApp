using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Sdk;

namespace IndaloAventurApi.IntegrationTests;

public sealed class InventarioSqlServerUniqueIndexesTests
{
    [Fact]
    [Trait("Category", "SqlServer")]
    public async Task IndicesUnicos_RechazanDuplicadosNormalizadosEnLaBaseDeDatos()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(typeof(Program).Assembly, optional: true)
            .Build();
        var configuredConnectionString = configuration.GetConnectionString("api_ContextConnection")
            ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            throw SkipException.ForSkip("La prueba de indices SQL Server requiere ConnectionStrings:api_ContextConnection en User Secrets.");
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder(configuredConnectionString)
        {
            InitialCatalog = $"indalo_inventory_index_tests_{Guid.NewGuid():N}"
        };
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionStringBuilder.ConnectionString)
            .Options;

        try
        {
            await using (var database = new ApplicationDbContext(options))
            {
                await database.Database.EnsureCreatedAsync();
            }

            await CrearEstadoAsync(options, "Disponible");
            await Assert.ThrowsAsync<DbUpdateException>(() => CrearEstadoAsync(options, " disponible "));

            var materialId = await CrearCategoriaAsync(options, "Material", null);
            await Assert.ThrowsAsync<DbUpdateException>(() => CrearCategoriaAsync(options, " material ", null));

            var ropaId = await CrearCategoriaAsync(options, "Ropa", null);
            await CrearCategoriaAsync(options, "Cascos", materialId);
            await CrearCategoriaAsync(options, "cascos", ropaId);
            await Assert.ThrowsAsync<DbUpdateException>(() => CrearCategoriaAsync(options, " Cascos ", materialId));
        }
        finally
        {
            await using var database = new ApplicationDbContext(options);
            await database.Database.EnsureDeletedAsync();
        }
    }

    private static async Task CrearEstadoAsync(DbContextOptions<ApplicationDbContext> options, string nombre)
    {
        await using var database = new ApplicationDbContext(options);
        database.InventarioEstados.Add(InventarioEstado.Crear(nombre));
        await database.SaveChangesAsync();
    }

    private static async Task<int> CrearCategoriaAsync(DbContextOptions<ApplicationDbContext> options, string nombre, int? categoriaPadreId)
    {
        await using var database = new ApplicationDbContext(options);
        var categoria = InventarioCategoria.Crear(nombre, categoriaPadreId);
        database.InventarioCategorias.Add(categoria);
        await database.SaveChangesAsync();
        return categoria.Id;
    }
}
