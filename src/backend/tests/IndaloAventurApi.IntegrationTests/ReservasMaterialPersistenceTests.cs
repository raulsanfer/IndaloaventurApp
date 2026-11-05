using System.Data;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.GetAllInventarioMateriales;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservasMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMisReservasMaterial;
using IndaloAventurApi.Domain.FichasSocio;
using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using IndaloAventurApi.Infrastructure.Persistence.Inventario;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.IntegrationTests;

public sealed class ReservasMaterialPersistenceTests
{
    [Fact]
    public async Task ProyeccionesDapper_AislanReservasPropiasYOrdenanSinMaterializarAgregados()
    {
        await using var fixture = await ReservaFixture.CreateAsync();
        await fixture.AddReservaAsync(fixture.UserOneId, 2, new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc));
        await fixture.AddReservaAsync(fixture.UserOneId, 1, new DateTime(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc));
        await fixture.AddReservaAsync(fixture.UserTwoId, 1, new DateTime(2026, 11, 3, 9, 0, 0, DateTimeKind.Utc));

        await using var readContext = fixture.CreateContext();
        var ownHandler = new GetMisReservasMaterialQueryHandler(
            new SqliteQueryConnectionFactory(fixture.ConnectionString),
            new SocioReservaReader(readContext));
        var own = await ownHandler.Handle(new GetMisReservasMaterialQuery(fixture.UserOneId, true), CancellationToken.None);

        Assert.Equal(2, own.Count);
        Assert.All(own, reserva => Assert.Equal(fixture.UserOneId, reserva.UserId));
        Assert.True(own.First().FechaSolicitudUtc > own.Last().FechaSolicitudUtc);
        Assert.All(own, reserva =>
        {
            Assert.NotNull(reserva.Socio);
            Assert.NotNull(reserva.Disponibilidad);
            Assert.Equal(1, reserva.Disponibilidad!.CantidadDisponible);
        });

        var adminHandler = new GetAdminReservasMaterialQueryHandler(new SqliteQueryConnectionFactory(fixture.ConnectionString));
        var all = await adminHandler.Handle(new GetAdminReservasMaterialQuery(true), CancellationToken.None);

        Assert.Equal(3, all.Count);
        Assert.All(all, reserva => Assert.NotNull(reserva.Socio));
    }

    [Fact]
    public async Task Repositorio_EvitaSobreasignarUnidadesCuandoDosSolicitudesCompiten()
    {
        await using var fixture = await ReservaFixture.CreateAsync();
        using var start = new Barrier(2);

        var attempts = await Task.WhenAll(
            IntentarCrearReservaAsync(fixture, fixture.UserOneId, 3, start),
            IntentarCrearReservaAsync(fixture, fixture.UserTwoId, 3, start));

        await using var verificationContext = fixture.CreateContext();
        var comprometidas = await verificationContext.ReservasMaterial
            .Where(x => x.InventarioMaterialId == fixture.MaterialId
                        && (x.Estado == EstadoReservaMaterial.Solicitada || x.Estado == EstadoReservaMaterial.Retirada))
            .SumAsync(x => (int?)x.CantidadSolicitada) ?? 0;

        Assert.Contains(attempts, result => result);
        Assert.True(comprometidas <= 5);
    }

    [Fact]
    public async Task Catalogo_ExponeLaCantidadDisponibleSinContarReservasFinalizadas()
    {
        await using var fixture = await ReservaFixture.CreateAsync();
        await fixture.AddReservaAsync(fixture.UserOneId, 2, new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc));
        await fixture.AddReservaAsync(fixture.UserTwoId, 1, new DateTime(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc));

        var handler = new GetAllInventarioMaterialesQueryHandler(new SqliteQueryConnectionFactory(fixture.ConnectionString));
        var material = Assert.Single(await handler.Handle(new GetAllInventarioMaterialesQuery(), CancellationToken.None));

        Assert.Equal(5, material.Cantidad);
        Assert.Equal(2, material.CantidadDisponible);
    }

    private static async Task<bool> IntentarCrearReservaAsync(ReservaFixture fixture, Guid userId, int cantidad, Barrier start)
    {
        try
        {
            await Task.Run(() => start.SignalAndWait());
            await using var context = fixture.CreateContext();
            var reservas = new ReservaMaterialRepository(context);
            var material = new InventarioMaterialRepository(context);

            var created = await reservas.ExecuteSerializableAsync(async cancellationToken =>
            {
                var item = await material.GetByIdForUpdateAsync(fixture.MaterialId, cancellationToken);
                var comprometidas = await reservas.GetCantidadComprometidaAsync(fixture.MaterialId, null, cancellationToken);
                if (item is null || comprometidas + cantidad > item.Cantidad)
                {
                    return false;
                }

                await reservas.AddAsync(
                    ReservaMaterial.Crear(userId, fixture.MaterialId, cantidad, new DateOnly(2026, 11, 4), new DateOnly(2026, 11, 6), null),
                    cancellationToken);
                await reservas.SaveChangesAsync(cancellationToken);
                return true;
            }, CancellationToken.None);

            return created;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    private sealed class ReservaFixture : IAsyncDisposable
    {
        private readonly string _databasePath;

        private ReservaFixture(string databasePath, Guid userOneId, Guid userTwoId, int materialId)
        {
            _databasePath = databasePath;
            UserOneId = userOneId;
            UserTwoId = userTwoId;
            MaterialId = materialId;
        }

        public string ConnectionString => $"Data Source={_databasePath};Default Timeout=1";
        public Guid UserOneId { get; }
        public Guid UserTwoId { get; }
        public int MaterialId { get; }

        public static async Task<ReservaFixture> CreateAsync()
        {
            var path = Path.Combine(Path.GetTempPath(), $"indalo-reservas-{Guid.NewGuid():N}.db");
            var connectionString = $"Data Source={path};Default Timeout=1";
            var userOneId = Guid.NewGuid();
            var userTwoId = Guid.NewGuid();

            await using var context = CreateContext(connectionString);
            await context.Database.EnsureCreatedAsync();
            context.Users.AddRange(
                CrearUsuario(userOneId, "socio-uno@club.test"),
                CrearUsuario(userTwoId, "socio-dos@club.test"));
            context.FichasSocio.AddRange(CrearFicha(userOneId, "Uno", "socio-uno@club.test"), CrearFicha(userTwoId, "Dos", "socio-dos@club.test"));
            var categoria = InventarioCategoria.Crear("Seguridad", null);
            var estado = InventarioEstado.Crear("Disponible");
            context.InventarioCategorias.Add(categoria);
            context.InventarioEstados.Add(estado);
            await context.SaveChangesAsync();

            var material = InventarioMaterial.Crear("Casco", null, 5, categoria.Id, estado.Id, null, "Comprado", null, null);
            context.InventarioMateriales.Add(material);
            await context.SaveChangesAsync();

            return new ReservaFixture(path, userOneId, userTwoId, material.Id);
        }

        public ApplicationDbContext CreateContext() => CreateContext(ConnectionString);

        public async Task AddReservaAsync(Guid userId, int cantidad, DateTime fechaSolicitudUtc)
        {
            await using var context = CreateContext();
            context.ReservasMaterial.Add(ReservaMaterial.Crear(
                userId,
                MaterialId,
                cantidad,
                new DateOnly(2026, 11, 4),
                new DateOnly(2026, 11, 6),
                null,
                fechaSolicitudUtc));
            await context.SaveChangesAsync();
        }

        public ValueTask DisposeAsync()
        {
            try
            {
                File.Delete(_databasePath);
                File.Delete($"{_databasePath}-shm");
                File.Delete($"{_databasePath}-wal");
            }
            catch
            {
                // La limpieza del archivo temporal no debe ocultar el resultado de la prueba.
            }

            return ValueTask.CompletedTask;
        }

        private static ApplicationDbContext CreateContext(string connectionString)
            => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connectionString).Options);

        private static Usuario CrearUsuario(Guid id, string email)
            => new()
            {
                Id = id,
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                IsMember = true
            };

        private static FichaSocio CrearFicha(Guid userId, string apellido, string email)
            => FichaSocio.Crear(
                userId,
                null,
                "Socio",
                apellido,
                "00000000T",
                new DateOnly(1990, 1, 1),
                "Calle Prueba 1",
                "04001",
                "Almeria",
                "Almeria",
                "600000000",
                email,
                null,
                true,
                true,
                false);
    }

    private sealed class SqliteQueryConnectionFactory(string connectionString) : IQueryConnectionFactory
    {
        public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}
