using IndaloAventurApi.Domain.LicenciasFederativas;
using IndaloAventurApi.Infrastructure.Persistence;
using IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.IntegrationTests;

public sealed class FederativeLicensePersistenceTests
{
    [Fact]
    public async Task CatalogoFederativo_ShouldSeed50TarifasFor2026()
    {
        await using var context = CreateContext(nameof(CatalogoFederativo_ShouldSeed50TarifasFor2026));

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var tarifas = await context.TarifasLicenciasFederativas
            .Where(x => x.Temporada == 2026)
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.Equal(50, tarifas.Count);
        Assert.Contains(tarifas, x => x.Licencia == "A NAC +" && x.Categoria == "Mayores" && !x.MediaTemporada && x.PrecioClub == 69m && x.PrecioIndependiente == 89m);
        Assert.Contains(tarifas, x => x.Licencia == "A NAC +" && x.Categoria == "Mayores" && x.MediaTemporada && x.PrecioClub == 69m && x.PrecioIndependiente == 89m);
    }

    [Fact]
    public void FederativeLicenseModel_ShouldDefineUniqueIndexes()
    {
        using var context = CreateContext(nameof(FederativeLicenseModel_ShouldDefineUniqueIndexes));

        var tarifaEntity = context.Model.FindEntityType(typeof(TarifaLicenciaFederativa));
        Assert.NotNull(tarifaEntity);
        Assert.Contains(
            tarifaEntity!.GetIndexes(),
            index => index.IsUnique && index.Properties.Select(x => x.Name).SequenceEqual(["Temporada", "Licencia", "Categoria", "MediaTemporada"]));

        var solicitudEntity = context.Model.FindEntityType(typeof(SolicitudLicenciaFederativa));
        Assert.NotNull(solicitudEntity);
        Assert.Contains(
            solicitudEntity!.GetIndexes(),
            index => index.IsUnique && index.Properties.Select(x => x.Name).SequenceEqual(["UserId", "Temporada"]));
    }

    [Fact]
    public async Task SolicitudLicenciaFederativaRepository_ShouldPreserveHistoricalRequestsByUser()
    {
        await using var context = CreateContext(nameof(SolicitudLicenciaFederativaRepository_ShouldPreserveHistoricalRequestsByUser));
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var tarifasRepo = new TarifaLicenciaFederativaRepository(context);
        var solicitudesRepo = new SolicitudLicenciaFederativaRepository(context);

        var tarifa2026 = await tarifasRepo.GetByIdAsync(1, CancellationToken.None);
        Assert.NotNull(tarifa2026);

        var tarifa2027 = TarifaLicenciaFederativa.Crear(2027, "A", "Mayores", 50m, 70m, "Andalucia, Ceuta y Melilla");
        await tarifasRepo.AddAsync(tarifa2027, CancellationToken.None);
        await tarifasRepo.SaveChangesAsync(CancellationToken.None);

        var userId = Guid.NewGuid();
        var solicitud2026 = SolicitudLicenciaFederativa.Crear(userId, 2026, tarifa2026!);
        var solicitud2027 = SolicitudLicenciaFederativa.Crear(userId, 2027, tarifa2027);

        await solicitudesRepo.AddAsync(solicitud2026, CancellationToken.None);
        await solicitudesRepo.AddAsync(solicitud2027, CancellationToken.None);
        await solicitudesRepo.SaveChangesAsync(CancellationToken.None);

        var historico = await solicitudesRepo.ListByUserIdAsync(userId, CancellationToken.None);

        Assert.Equal(2, historico.Count);
        Assert.Contains(historico, x => x.Temporada == 2026 && x.TarifaLicenciaFederativa!.Temporada == 2026);
        Assert.Contains(historico, x => x.Temporada == 2027 && x.TarifaLicenciaFederativa!.Temporada == 2027);
    }

    private static ApplicationDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApplicationDbContext(options);
    }
}
