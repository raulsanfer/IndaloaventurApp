using System.Reflection;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CancelMiReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CancelAdminReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarMiDevolucionMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarDevolucionAdministracion;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CreateMiReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMiReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.RegistrarRetiradaReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.UpdateMiReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.UpdateInventarioMaterial;
using IndaloAventurApi.Domain.Abstractions;
using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Tests.Inventario;

public sealed class ReservasMaterialHandlersTests
{
    private static readonly IReservaMaterialNotificationService NotificationService = new NoOpReservaMaterialNotificationService();

    [Fact]
    public async Task Create_ReservaValida_AlmacenaUnidadesDisponibles()
    {
        var userId = Guid.NewGuid();
        var material = CrearMaterial(5, activo: true);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(CrearReserva(Guid.NewGuid(), material.Id, 2), CancellationToken.None);
        var handler = new CreateMiReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(userId),
            NotificationService);

        var result = await handler.Handle(CrearComando(userId, material.Id, 3), CancellationToken.None);

        Assert.Equal(3, result.CantidadSolicitada);
        Assert.Equal(EstadoReservaMaterial.Solicitada, result.Estado);
        Assert.Equal(2, reservas.Items.Count);
    }

    [Fact]
    public async Task Create_RechazaUsuarioSinCondicionDeSocioOSinFicha()
    {
        var userId = Guid.NewGuid();
        var material = CrearMaterial(1, activo: true);
        var repository = new InMemoryReservaMaterialRepository();
        var materiales = new InMemoryInventarioMaterialRepository([material]);

        var noSocio = new CreateMiReservaMaterialCommandHandler(repository, materiales, new FakeSocioReservaReader(userId), NotificationService);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => noSocio.Handle(CrearComando(userId, material.Id, 1, isMember: false), CancellationToken.None));

        var sinFicha = new CreateMiReservaMaterialCommandHandler(repository, materiales, new FakeSocioReservaReader(), NotificationService);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => sinFicha.Handle(CrearComando(userId, material.Id, 1), CancellationToken.None));
    }

    [Fact]
    public async Task Create_RechazaMaterialInactivoYCantidadExcedida()
    {
        var userId = Guid.NewGuid();
        var inactivo = CrearMaterial(2, activo: false);
        var handlerInactivo = new CreateMiReservaMaterialCommandHandler(
            new InMemoryReservaMaterialRepository(),
            new InMemoryInventarioMaterialRepository([inactivo]),
            new FakeSocioReservaReader(userId),
            NotificationService);

        var inactivoException = await Assert.ThrowsAsync<InvalidOperationException>(() => handlerInactivo.Handle(CrearComando(userId, inactivo.Id, 1), CancellationToken.None));
        Assert.Equal("El material de inventario no esta activo para nuevas reservas.", inactivoException.Message);

        var activo = CrearMaterial(3, activo: true);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(CrearReserva(Guid.NewGuid(), activo.Id, 2), CancellationToken.None);
        var handlerExcedida = new CreateMiReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([activo]),
            new FakeSocioReservaReader(userId),
            NotificationService);

        var excedidaException = await Assert.ThrowsAsync<InvalidOperationException>(() => handlerExcedida.Handle(CrearComando(userId, activo.Id, 2), CancellationToken.None));
        Assert.Equal("No hay unidades disponibles suficientes para esta reserva.", excedidaException.Message);
    }

    [Fact]
    public async Task Create_RechazaFechasInvalidasSinPersistirReserva()
    {
        var userId = Guid.NewGuid();
        var material = CrearMaterial(1, activo: true);
        var reservas = new InMemoryReservaMaterialRepository();
        var handler = new CreateMiReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(userId),
            NotificationService);
        var retirada = new DateOnly(2026, 11, 4);

        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CreateMiReservaMaterialCommand(userId, true, material.Id, 1, retirada, retirada.AddDays(-1), null),
            CancellationToken.None));

        Assert.Empty(reservas.Items);
    }

    [Fact]
    public async Task Create_PersisteAntesDeNotificarYUnFalloDeCorreoNoRevierteLaReserva()
    {
        var userId = Guid.NewGuid();
        var material = CrearMaterial(1, activo: true);
        var reservas = new InMemoryReservaMaterialRepository();
        var notificationService = new ReservaMaterialNotificationService(
            new ThrowingEmailSender(),
            Options.Create(new ReservaMaterialNotificationOptions { InventoryReservationNotificationsTo = "administracion@club.test" }),
            NullLogger<ReservaMaterialNotificationService>.Instance);
        var handler = new CreateMiReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(userId),
            notificationService);

        await handler.Handle(CrearComando(userId, material.Id, 1), CancellationToken.None);

        Assert.Single(reservas.Items);
        Assert.Equal(EstadoReservaMaterial.Solicitada, reservas.Items[0].Estado);
    }

    [Fact]
    public async Task Update_ReservaPropiaSolicitada_ExcluyeSuCantidadAlRecalcularDisponibilidad()
    {
        var userId = Guid.NewGuid();
        var material = CrearMaterial(3, activo: true);
        var reserva = CrearReserva(userId, material.Id, 1);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var handler = new UpdateMiReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(userId));

        var result = await handler.Handle(new UpdateMiReservaMaterialCommand(
            userId,
            true,
            reserva.Id,
            material.Id,
            3,
            new DateOnly(2026, 11, 5),
            new DateOnly(2026, 11, 7),
            "Actualizada"), CancellationToken.None);

        Assert.Equal(3, result.CantidadSolicitada);
        Assert.Equal("Actualizada", result.Observaciones);
    }

    [Fact]
    public async Task Update_RechazaReservaAjenaORetirada()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var material = CrearMaterial(2, activo: true);
        var reserva = CrearReserva(ownerId, material.Id, 1);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var handler = new UpdateMiReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(ownerId, otherUserId));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(CrearActualizacion(otherUserId, reserva, material.Id), CancellationToken.None));

        reserva.RegistrarRetirada(new DateOnly(2026, 11, 4));
        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(CrearActualizacion(ownerId, reserva, material.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Get_ReservaPropia_SeAislaDeLasReservasAjenas()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var material = CrearMaterial(1, activo: false);
        var reserva = CrearReserva(ownerId, material.Id, 1);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var handler = new GetMiReservaMaterialQueryHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(ownerId, otherUserId));

        var result = await handler.Handle(new GetMiReservaMaterialQuery(ownerId, true, reserva.Id), CancellationToken.None);

        Assert.Equal(reserva.Id, result.Id);
        Assert.Equal("Casco", result.MaterialDescripcion);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new GetMiReservaMaterialQuery(otherUserId, true, reserva.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Cancel_SoloPermiteReservasPropiasSolicitadas()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var material = CrearMaterial(1, activo: true);
        var reserva = CrearReserva(ownerId, material.Id, 1);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var handler = new CancelMiReservaMaterialCommandHandler(
            reservas,
            new FakeSocioReservaReader(ownerId, otherUserId),
            new InMemoryInventarioMaterialRepository([material]),
            NotificationService);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new CancelMiReservaMaterialCommand(otherUserId, true, reserva.Id), CancellationToken.None));

        var cancelled = await handler.Handle(new CancelMiReservaMaterialCommand(ownerId, true, reserva.Id), CancellationToken.None);

        Assert.True(cancelled);
        Assert.Equal(EstadoReservaMaterial.Cancelada, reserva.Estado);
        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CancelMiReservaMaterialCommand(ownerId, true, reserva.Id), CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmarDevolucionSocio_SoloPermiteReservaPropiaRetiradaYPuedeCompletarla()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var material = CrearMaterial(1, activo: true);
        var reserva = CrearReserva(ownerId, material.Id, 1);
        reserva.RegistrarRetirada(new DateOnly(2026, 11, 4));
        reserva.ConfirmarDevolucionAdministracion(new DateOnly(2026, 11, 6));
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var handler = new ConfirmarMiDevolucionMaterialCommandHandler(
            reservas,
            new FakeSocioReservaReader(ownerId, otherUserId),
            new InMemoryInventarioMaterialRepository([material]),
            NotificationService);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new ConfirmarMiDevolucionMaterialCommand(otherUserId, true, reserva.Id), CancellationToken.None));

        var confirmed = await handler.Handle(
            new ConfirmarMiDevolucionMaterialCommand(ownerId, true, reserva.Id), CancellationToken.None);

        Assert.True(confirmed);
        Assert.True(reserva.ConfirmacionDevolucionSocio);
        Assert.Equal(EstadoReservaMaterial.Entregada, reserva.Estado);
    }

    [Fact]
    public async Task Administracion_PuedeConsultarYRegistrarRetirada_SoloConPermisoAdmin()
    {
        var ownerId = Guid.NewGuid();
        var material = CrearMaterial(1, activo: true);
        var reserva = CrearReserva(ownerId, material.Id, 1);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var retiradaHandler = new RegistrarRetiradaReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(ownerId),
            NotificationService);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => retiradaHandler.Handle(
            new RegistrarRetiradaReservaMaterialCommand(false, reserva.Id, new DateOnly(2026, 11, 4)), CancellationToken.None));

        await retiradaHandler.Handle(
            new RegistrarRetiradaReservaMaterialCommand(true, reserva.Id, new DateOnly(2026, 11, 4)), CancellationToken.None);

        Assert.Equal(EstadoReservaMaterial.Retirada, reserva.Estado);
        Assert.Equal(new DateOnly(2026, 11, 4), reserva.FechaRetirada);

        var queryHandler = new GetAdminReservaMaterialQueryHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(ownerId));
        var detail = await queryHandler.Handle(new GetAdminReservaMaterialQuery(true, reserva.Id), CancellationToken.None);

        Assert.Equal(ownerId, detail.Socio!.UserId);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => queryHandler.Handle(
            new GetAdminReservaMaterialQuery(false, reserva.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Administracion_CancelacionDeRetiradaExigeObservacionYLiberaUnidades()
    {
        var material = CrearMaterial(2, activo: true);
        var reserva = CrearReserva(Guid.NewGuid(), material.Id, 2);
        reserva.RegistrarRetirada(new DateOnly(2026, 11, 4));
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var handler = new CancelAdminReservaMaterialCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(reserva.UserId),
            NotificationService);

        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CancelAdminReservaMaterialCommand(true, reserva.Id, " "), CancellationToken.None));

        await handler.Handle(
            new CancelAdminReservaMaterialCommand(true, reserva.Id, "Material no devuelto"), CancellationToken.None);

        Assert.Equal(EstadoReservaMaterial.Cancelada, reserva.Estado);
        Assert.Equal(0, await reservas.GetCantidadComprometidaAsync(material.Id, null, CancellationToken.None));
    }

    [Fact]
    public async Task Administracion_ConfirmaDevolucionConFechaEfectivaYCompletaCuandoElSocioConfirmo()
    {
        var material = CrearMaterial(1, activo: true);
        var reserva = CrearReserva(Guid.NewGuid(), material.Id, 1);
        reserva.RegistrarRetirada(new DateOnly(2026, 11, 4));
        reserva.ConfirmarDevolucionSocio();
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(reserva, CancellationToken.None);
        var handler = new ConfirmarDevolucionAdministracionCommandHandler(
            reservas,
            new InMemoryInventarioMaterialRepository([material]),
            new FakeSocioReservaReader(reserva.UserId),
            NotificationService);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(
            new ConfirmarDevolucionAdministracionCommand(false, reserva.Id, new DateOnly(2026, 11, 6)), CancellationToken.None));

        await handler.Handle(
            new ConfirmarDevolucionAdministracionCommand(true, reserva.Id, new DateOnly(2026, 11, 6)), CancellationToken.None);

        Assert.Equal(EstadoReservaMaterial.Entregada, reserva.Estado);
        Assert.Equal(new DateOnly(2026, 11, 6), reserva.FechaDevolucionEfectiva);
        Assert.Equal(0, await reservas.GetCantidadComprometidaAsync(material.Id, null, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateMaterial_NoPermiteReducirStockPorDebajoDeReservasActivas()
    {
        var material = CrearMaterial(5, activo: true);
        var reservas = new InMemoryReservaMaterialRepository();
        await reservas.AddAsync(CrearReserva(Guid.NewGuid(), material.Id, 3), CancellationToken.None);
        var handler = new UpdateInventarioMaterialCommandHandler(
            new InMemoryInventarioMaterialRepository([material]),
            new ExistingCategoriaRepository(),
            new ExistingEstadoRepository(),
            reservas);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new UpdateInventarioMaterialCommand(
                material.Id,
                "Casco",
                null,
                2,
                1,
                1,
                null,
                "Comprado",
                null,
                null,
                true),
            CancellationToken.None));

        Assert.Equal("No se puede reducir la cantidad por debajo de las unidades comprometidas en reservas activas.", exception.Message);
        Assert.Equal(5, material.Cantidad);
    }

    private static CreateMiReservaMaterialCommand CrearComando(Guid userId, int materialId, int cantidad, bool isMember = true)
        => new(userId, isMember, materialId, cantidad, new DateOnly(2026, 11, 4), new DateOnly(2026, 11, 6), "Uso del club");

    private static UpdateMiReservaMaterialCommand CrearActualizacion(Guid userId, ReservaMaterial reserva, int materialId)
        => new(userId, true, reserva.Id, materialId, 1, new DateOnly(2026, 11, 5), new DateOnly(2026, 11, 7), null);

    private static ReservaMaterial CrearReserva(Guid userId, int materialId, int cantidad)
        => ReservaMaterial.Crear(userId, materialId, cantidad, new DateOnly(2026, 11, 4), new DateOnly(2026, 11, 6), null);

    private static InventarioMaterial CrearMaterial(int cantidad, bool activo)
    {
        var material = InventarioMaterial.Crear("Casco", null, cantidad, 1, 1, null, "Comprado", null, null, activo);
        InMemoryInventarioMaterialRepository.AssignId(material, Guid.NewGuid().GetHashCode() & int.MaxValue);
        return material;
    }

    private sealed class InMemoryReservaMaterialRepository : IReservaMaterialRepository
    {
        public List<ReservaMaterial> Items { get; } = [];

        public Task<ReservaMaterial?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(Items.SingleOrDefault(x => x.Id == id));

        public Task<ReservaMaterial?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken)
            => GetByIdAsync(id, cancellationToken);

        public Task<int> GetCantidadComprometidaAsync(int inventarioMaterialId, Guid? excludingReservaId, CancellationToken cancellationToken)
            => Task.FromResult(Items
                .Where(x => x.InventarioMaterialId == inventarioMaterialId
                            && x.Id != excludingReservaId
                            && x.Estado is EstadoReservaMaterial.Solicitada or EstadoReservaMaterial.Retirada)
                .Sum(x => x.CantidadSolicitada));

        public Task AddAsync(ReservaMaterial reserva, CancellationToken cancellationToken)
        {
            Items.Add(reserva);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<T> ExecuteSerializableAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
            => operation(cancellationToken);
    }

    private sealed class InMemoryInventarioMaterialRepository(IEnumerable<InventarioMaterial> materials) : IInventarioMaterialRepository
    {
        private static readonly FieldInfo IdField = typeof(InventarioMaterial).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private readonly Dictionary<int, InventarioMaterial> _items = materials.ToDictionary(x => x.Id);

        public static void AssignId(InventarioMaterial material, int id) => IdField.SetValue(material, id);
        public Task AddAsync(InventarioMaterial material, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<InventarioMaterial?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.TryGetValue(id, out var material) ? material : null);
        public Task<InventarioMaterial?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken)
            => GetByIdAsync(id, cancellationToken);
        public Task<bool> ExistsWithNormalizedSerialAsync(string normalizedSerial, int? excludingId, CancellationToken cancellationToken)
            => Task.FromResult(false);
        public void Remove(InventarioMaterial material) { }
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakeSocioReservaReader(params Guid[] userIds) : ISocioReservaReader
    {
        private readonly HashSet<Guid> _userIds = userIds.ToHashSet();

        public Task<SocioReservaDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(
                _userIds.Contains(userId)
                    ? new SocioReservaDto(userId, "Socio de prueba", "socio@club.test")
                    : null);
    }

    private sealed class ExistingCategoriaRepository : IInventarioCategoriaRepository
    {
        private readonly InventarioCategoria _categoria = InventarioCategoria.Crear("Categoria", null);

        public Task AddAsync(InventarioCategoria categoria, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<InventarioCategoria?> GetByIdAsync(int id, CancellationToken cancellationToken) => Task.FromResult<InventarioCategoria?>(_categoria);
        public Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? categoriaPadreId, int? excludingId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken) => Task.FromResult(false);
        public void Remove(InventarioCategoria categoria) { }
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class ExistingEstadoRepository : IInventarioEstadoRepository
    {
        private readonly InventarioEstado _estado = InventarioEstado.Crear("Estado");

        public Task AddAsync(InventarioEstado estado, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<InventarioEstado?> GetByIdAsync(int id, CancellationToken cancellationToken) => Task.FromResult<InventarioEstado?>(_estado);
        public Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? excludingId, CancellationToken cancellationToken) => Task.FromResult(false);
        public void Remove(InventarioEstado estado) { }
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class NoOpReservaMaterialNotificationService : IReservaMaterialNotificationService
    {
        public Task NotifyAsync(ReservaMaterialNotification notification, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class ThrowingEmailSender : IEmailSender
    {
        public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
            => throw new InvalidOperationException("Proveedor de correo no disponible.");
    }
}
