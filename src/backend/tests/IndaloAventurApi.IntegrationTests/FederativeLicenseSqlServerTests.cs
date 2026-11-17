using System.Data;
using Dapper;
using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetMiSolicitudLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetMisSolicitudesLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetAdminSolicitudesLicenciaFederativa;
using IndaloAventurApi.Domain.LicenciasFederativas;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit.Sdk;

namespace IndaloAventurApi.IntegrationTests;

[Trait("Category", "SqlServer")]
public sealed class FederativeLicenseSqlServerTests
{
    private const string PreviousMigration = "20261029120150_AddInventoryMaterialReservations";
    private static readonly CancellationToken Token = CancellationToken.None;

    [Fact]
    public async Task ScriptsIdempotentes_SeEjecutanDosVecesEnAmbosSentidos()
    {
        await using var database = await IsolatedDatabase.CreateAsync(previous: true);
        await using var db = database.Context();
        var migrator = db.GetService<IMigrator>();
        const string target = "20261116165233_AddFederativeLicenseTypes";
        foreach (var script in new[]
        {
            migrator.GenerateScript(PreviousMigration, target, MigrationsSqlGenerationOptions.Idempotent),
            migrator.GenerateScript(target, PreviousMigration, MigrationsSqlGenerationOptions.Idempotent)
        })
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                await using var connection = new SqlConnection(database.ConnectionString);
                await connection.OpenAsync();
                foreach (var batch in System.Text.RegularExpressions.Regex.Split(script, @"^GO\s*$", System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    if (!string.IsNullOrWhiteSpace(batch)) await connection.ExecuteAsync(batch);
            }
        }
        await using var finalConnection = new SqlConnection(database.ConnectionString);
        Assert.Equal(50, await finalConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TarifasLicenciasFederativas"));
    }

    [Fact]
    public async Task Migracion_ConservaTodasLasTarifasYElHistoricoYAdmiteReversionCompatible()
    {
        await using var database = await IsolatedDatabase.CreateAsync(previous: true);
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("""
            UPDATE TarifasLicenciasFederativas SET PrecioClub = 123.45, PrecioIndependiente = NULL WHERE Id = 1;
            INSERT INTO TarifasLicenciasFederativas (Temporada, Licencia, Categoria, PrecioClub, PrecioIndependiente, Territorio, MediaTemporada)
            VALUES (2028, N'PERSONALIZADA', N'Mayores', 88.12, 99.34, N'Territorio personalizado', 0),
                   (2028, N'PERSONALIZADA', N'Mayores', 43.21, NULL, N'Territorio personalizado', 1);
            """);
        for (var estado = 1; estado <= 3; estado++)
            await connection.ExecuteAsync("""
                INSERT INTO SolicitudesLicenciasFederativas (Id, UserId, Temporada, TarifaLicenciaFederativaId, Estado, FechaCreacionUtc)
                VALUES (@Id, @UserId, 2026, @Tarifa, @Estado, @Creacion);
                """, new { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Tarifa = estado == 1 ? 26 : 1, Estado = estado, Creacion = new DateTime(2026, 2, 3, 10, 11, 12, DateTimeKind.Utc) });

        const string rateColumns = "Id, Temporada, Licencia, Categoria, PrecioClub, PrecioIndependiente, Territorio";
        var before = (await connection.QueryAsync<RateRow>($"SELECT {rateColumns}, MediaTemporada FROM TarifasLicenciasFederativas ORDER BY Id")).ToArray();
        var requestsBefore = (await connection.QueryAsync<RequestRow>("SELECT Id, UserId, Temporada, TarifaLicenciaFederativaId, Estado, FechaCreacionUtc FROM SolicitudesLicenciasFederativas ORDER BY Id")).ToArray();
        await using (var db = database.Context()) await db.Database.MigrateAsync();
        var after = (await connection.QueryAsync<RateRow>($"SELECT {rateColumns}, CAST(CASE WHEN TipoLicenciaId = 2 THEN 1 ELSE 0 END AS bit) AS MediaTemporada FROM TarifasLicenciasFederativas ORDER BY Id")).ToArray();
        Assert.Equal(before, after);
        Assert.Equal(requestsBefore, (await connection.QueryAsync<RequestRow>("SELECT Id, UserId, Temporada, TarifaLicenciaFederativaId, Estado, FechaCreacionUtc FROM SolicitudesLicenciasFederativas ORDER BY Id")).ToArray());
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM SolicitudesLicenciasFederativas WHERE FechaInicio IS NOT NULL OR FechaFin IS NOT NULL"));
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TarifasLicenciasFederativas WHERE TipoLicenciaId = 3"));
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('TarifasLicenciasFederativas') AND name = 'MediaTemporada'"));
        Assert.Equal(3, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TiposLicencias"));
        var queries = new SqlConnections(database.ConnectionString);
        var historical = await new GetMisSolicitudesLicenciaFederativaQueryHandler(queries).Handle(new(requestsBefore[0].UserId), Token);
        Assert.Null(Assert.Single(historical).FechaInicio);

        await using (var db = database.Context()) await db.Database.MigrateAsync(); // Repeated deployment is a no-op.
        await using (var db = database.Context()) await db.GetService<IMigrator>().MigrateAsync(PreviousMigration);
        Assert.Equal(before, (await connection.QueryAsync<RateRow>($"SELECT {rateColumns}, MediaTemporada FROM TarifasLicenciasFederativas ORDER BY Id")).ToArray());
        Assert.Equal(requestsBefore.Length, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM SolicitudesLicenciasFederativas"));
        await using (var db = database.Context()) await db.Database.MigrateAsync();
    }

    [Theory]
    [InlineData("tipo")]
    [InlineData("fechas")]
    [InlineData("multiplicidad")]
    public async Task Reversion_IncompatibleAbortaSinModificarDatos(string incompatibilidad)
    {
        await using var database = await IsolatedDatabase.CreateAsync();
        await using (var db = database.Context())
        {
            if (incompatibilidad == "tipo")
                db.Add(TarifaLicenciaFederativa.Crear(2026, "D", "Mayores", 5m, null, "Andalucía", 3));
            else
            {
                var tarifa = await db.TarifasLicenciasFederativas.FirstAsync();
                var user = Guid.NewGuid();
                db.Add(SolicitudLicenciaFederativa.Crear(user, tarifa.Temporada, tarifa,
                    incompatibilidad == "fechas" ? new DateOnly(2026, 1, 1) : null,
                    incompatibilidad == "fechas" ? new DateOnly(2026, 1, 1) : null));
                if (incompatibilidad == "multiplicidad") db.Add(SolicitudLicenciaFederativa.Crear(user, tarifa.Temporada, tarifa));
            }
            await db.SaveChangesAsync();
        }
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        var ratesBefore = await connection.ExecuteScalarAsync<string>("SELECT * FROM TarifasLicenciasFederativas ORDER BY Id FOR JSON PATH, INCLUDE_NULL_VALUES");
        var requestsBefore = await connection.ExecuteScalarAsync<string>("SELECT * FROM SolicitudesLicenciasFederativas ORDER BY Id FOR JSON PATH, INCLUDE_NULL_VALUES");
        await using (var db = database.Context())
        {
            var exception = await Assert.ThrowsAsync<SqlException>(() => db.GetService<IMigrator>().MigrateAsync(PreviousMigration));
            Assert.Equal(51001, exception.Number);
        }
        Assert.Equal(ratesBefore, await connection.ExecuteScalarAsync<string>("SELECT * FROM TarifasLicenciasFederativas ORDER BY Id FOR JSON PATH, INCLUDE_NULL_VALUES"));
        Assert.Equal(requestsBefore, await connection.ExecuteScalarAsync<string>("SELECT * FROM SolicitudesLicenciasFederativas ORDER BY Id FOR JSON PATH, INCLUDE_NULL_VALUES"));
        Assert.Equal(3, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TiposLicencias"));
    }

    [Fact]
    public async Task Persistencia_ImpideTiposDuplicadosTarifasDuplicadasYReferenciasInvalidas()
    {
        await using var database = await IsolatedDatabase.CreateAsync();
        await using (var db = database.Context())
        {
            Assert.Equal(50, await db.TarifasLicenciasFederativas.CountAsync());
            Assert.Equal(new[] { "Completa", "Media 01/07 - 31/12", "Día" }, await db.TiposLicencias.OrderBy(x => x.Id).Select(x => x.Nombre).ToArrayAsync());
            db.Add(TipoLicencia.Crear(4, "Completa"));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
        await using (var db = database.Context())
        {
            db.Add(TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 10m, null, "Andalucía", 1));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
        await using (var db = database.Context())
        {
            db.Add(TarifaLicenciaFederativa.Crear(2026, "OTRA", "Mayores", 10m, null, "Andalucía", 999));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
        await using (var db = database.Context())
        {
            db.Add(TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 10m, null, "Andalucía", 3));
            await db.SaveChangesAsync();
            Assert.Equal(3, await db.TarifasLicenciasFederativas.CountAsync(x => x.Licencia == "A" && x.Categoria == "Mayores"));
        }
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        foreach (var interval in new[] { ("2026-05-01", (string?)null), ("2026-05-02", "2026-05-01"), ("2027-05-01", "2027-05-02") })
            await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync("""
                INSERT INTO SolicitudesLicenciasFederativas (Id, UserId, Temporada, TarifaLicenciaFederativaId, Estado, FechaCreacionUtc, FechaInicio, FechaFin)
                VALUES (NEWID(), NEWID(), 2026, 1, 0, SYSUTCDATETIME(), @Inicio, @Fin)
                """, new { Inicio = interval.Item1, Fin = interval.Item2 }));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AltasNoDiariasConcurrentes_RespetanExclusividadEnRangoVacioYOcupado(bool conDiariaPrevia)
    {
        await using var database = await IsolatedDatabase.CreateAsync();
        var user = Guid.NewGuid();
        var diaId = await AddDayRate(database);
        if (conDiariaPrevia) await Create(database, user, diaId, new DateOnly(2026, 1, 1));
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = new[] { 1, 26 }.Select(id => Task.Run(async () =>
        {
            await gate.Task;
            try { await Create(database, user, id); return true; }
            catch (InvalidOperationException e) when (e.Message.Contains("completa o media")) { return false; }
        })).ToArray();
        gate.SetResult();
        var results = await Task.WhenAll(tasks);
        Assert.Single(results, x => x);
        Assert.Single(results, x => !x);
        await using var db = database.Context();
        Assert.Equal(conDiariaPrevia ? 2 : 1, await db.SolicitudesLicenciasFederativas.CountAsync());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AltasDiariasConcurrentes_ConvivenEntreSiYConNoDiariaYMaterializanFechas(bool mezclarNoDiaria)
    {
        await using var database = await IsolatedDatabase.CreateAsync();
        var diaId = await AddDayRate(database);
        var user = Guid.NewGuid();
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = Task.Run(async () => { await gate.Task; return await Create(database, user, diaId, new DateOnly(2026, 4, 1)); });
        var second = Task.Run(async () => { await gate.Task; return await Create(database, user, mezclarNoDiaria ? 1 : diaId, mezclarNoDiaria ? null : new DateOnly(2026, 5, 1)); });
        gate.SetResult();
        var created = await Task.WhenAll(first, second);
        var connections = new SqlConnections(database.ConnectionString);
        var own = await new GetMisSolicitudesLicenciaFederativaQueryHandler(connections).Handle(new(user), Token);
        Assert.Equal(2, own.Count);
        var daily = own.Single(x => x.Id == created[0].Id);
        Assert.Equal("Día", daily.TipoLicenciaNombre);
        Assert.Equal(new DateOnly(2026, 4, 1), daily.FechaInicio);
        Assert.Equal(daily.FechaInicio, daily.FechaFin);
        Assert.Equal(daily, await new GetMiSolicitudLicenciaFederativaQueryHandler(connections).Handle(new(user, daily.Id), Token));
        var admin = await new GetAdminSolicitudesLicenciaFederativaQueryHandler(connections).Handle(new(user, 2026, null), Token);
        Assert.Equal(2, admin.Count);
        Assert.Equal(daily.FechaInicio, admin.Single(x => x.Id == daily.Id).FechaInicio);
    }

    private static async Task<int> AddDayRate(IsolatedDatabase database)
    {
        await using var db = database.Context();
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "D", "Mayores", 5m, null, "Andalucía", 3);
        db.Add(tarifa);
        await db.SaveChangesAsync();
        return tarifa.Id;
    }

    private static async Task<SolicitudLicenciaFederativaDto> Create(IsolatedDatabase database, Guid user, int rate, DateOnly? date = null)
    {
        await using var db = database.Context();
        var handler = new CreateSolicitudLicenciaFederativaCommandHandler(new TarifaLicenciaFederativaRepository(db),
            new SolicitudLicenciaFederativaRepository(db), new NoEmail(), Options.Create(new FederativeLicenseRequestNotificationOptions()),
            NullLogger<CreateSolicitudLicenciaFederativaCommandHandler>.Instance);
        return await handler.Handle(new(user, "socio@club.test", true, 2026, rate, date, date), Token);
    }

    private sealed class NoEmail : IEmailSender
    {
        public Task SendAsync(EmailMessage message, CancellationToken token) => Task.CompletedTask;
    }

    private sealed class SqlConnections(string connectionString) : IQueryConnectionFactory
    {
        public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }

    private sealed record RateRow(int Id, int Temporada, string Licencia, string Categoria, decimal PrecioClub, decimal? PrecioIndependiente, string Territorio, bool MediaTemporada);
    private sealed record RequestRow(Guid Id, Guid UserId, int Temporada, int TarifaLicenciaFederativaId, int Estado, DateTime FechaCreacionUtc);

    private sealed class IsolatedDatabase(string connectionString) : IAsyncDisposable
    {
        private const string Prefix = "indalo_license_tests_";
        public string ConnectionString { get; } = connectionString;
        public ApplicationDbContext Context() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(ConnectionString).Options);

        public static async Task<IsolatedDatabase> CreateAsync(bool previous = false)
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly, optional: true).AddEnvironmentVariables().Build();
            var configured = configuration.GetConnectionString("api_ContextConnection") ?? configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(configured))
                throw SkipException.ForSkip("La prueba requiere una conexión SQL Server con permiso para crear una base aislada.");
            var connection = new SqlConnectionStringBuilder(configured) { InitialCatalog = Prefix + Guid.NewGuid().ToString("N") };
            var database = new IsolatedDatabase(connection.ConnectionString);
            try
            {
                await using var db = database.Context();
                if (previous) await db.GetService<IMigrator>().MigrateAsync(PreviousMigration);
                else await db.Database.MigrateAsync();
                return database;
            }
            catch
            {
                await database.DisposeAsync();
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            var target = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
            if (!target.StartsWith(Prefix, StringComparison.Ordinal) || target.Length != Prefix.Length + 32)
                throw new InvalidOperationException("La limpieza solo admite la base de pruebas aislada.");
            await using var db = Context();
            await db.Database.EnsureDeletedAsync();
        }
    }
}
