using System.Reflection;
using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignal;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignalComment;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalImages;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalComments;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.SearchSignals;
using IndaloAventurApi.Application.Features.TrailSignals.Signals.UpdateSignal;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.CreateSignalType;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.DeleteSignalType;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.GetAllSignalTypes;
using IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.UpdateSignalType;
using IndaloAventurApi.Domain.TrailSignals;

namespace IndaloAventurApi.Application.Tests.TrailSignals;

public sealed class TrailSignalsHandlersTests
{
    [Fact]
    public async Task CreateSignalType_ShouldPersistAndReturnId()
    {
        var repo = new InMemorySignalTypeRepository();
        var handler = new CreateSignalTypeCommandHandler(repo);

        var id = await handler.Handle(new CreateSignalTypeCommand("Incidencia", "alerta"), CancellationToken.None);

        Assert.Equal(1, id);
        Assert.NotNull(await repo.GetByIdAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateSignalType_ShouldThrowWhenMissing()
    {
        var repo = new InMemorySignalTypeRepository();
        var handler = new UpdateSignalTypeCommandHandler(repo);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new UpdateSignalTypeCommand(99, "N", "I"), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteSignalType_ShouldThrowWhenMissing()
    {
        var repo = new InMemorySignalTypeRepository();
        var handler = new DeleteSignalTypeCommandHandler(repo);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new DeleteSignalTypeCommand(99), CancellationToken.None));
    }

    [Fact]
    public async Task GetAllSignalTypes_ShouldReturnAllItems()
    {
        var repo = new InMemorySignalTypeRepository();
        var incidencia = SignalType.Crear("Incidencia", "alerta");
        await repo.AddAsync(incidencia, CancellationToken.None);
        var aviso = SignalType.Crear("Aviso", "aviso");
        await repo.AddAsync(aviso, CancellationToken.None);
        var handler = new GetAllSignalTypesQueryHandler(repo);

        var result = await handler.Handle(new GetAllSignalTypesQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, item => item.Id == aviso.Id && item.Nombre == "Aviso");
        Assert.Contains(result, item => item.Id == incidencia.Id && item.Nombre == "Incidencia");
    }

    [Fact]
    public async Task GetAllSignalTypes_WhenEmpty_ShouldReturnEmptyCollection()
    {
        var repo = new InMemorySignalTypeRepository();
        var handler = new GetAllSignalTypesQueryHandler(repo);

        var result = await handler.Handle(new GetAllSignalTypesQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateSignal_ShouldThrowWhenTypeDoesNotExist()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var handler = new CreateSignalCommandHandler(signalRepo, typeRepo);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new CreateSignalCommand(
            37.1f, -2.2f, "Sendero con piedras", "Piedras en camino", [1], [2], true, Guid.NewGuid(), 999, "piedras"), CancellationToken.None));
    }

    [Fact]
    public async Task CreateSignal_ShouldAllowMissingSecondPhoto()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var handler = new CreateSignalCommandHandler(signalRepo, typeRepo);

        var signalId = await handler.Handle(new CreateSignalCommand(1, 1, "Solo una foto", "Desc", [1], null, true, Guid.NewGuid(), aviso.Id, "tag"), CancellationToken.None);

        var signal = await signalRepo.GetByIdAsync(signalId, CancellationToken.None);
        Assert.NotNull(signal);
        Assert.Equal([1], signal!.Foto1);
        Assert.Empty(signal.Foto2);
    }

    [Fact]
    public async Task SearchSignals_ShouldFilterByTipo()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var incidencia = SignalType.Crear("Incidencia", "i2");
        await typeRepo.AddAsync(incidencia, CancellationToken.None);
        var createHandler = new CreateSignalCommandHandler(signalRepo, typeRepo);

        await createHandler.Handle(new CreateSignalCommand(1, 1, "Aviso uno", "Uno", [1], [11], true, Guid.NewGuid(), aviso.Id, "tag1"), CancellationToken.None);
        await createHandler.Handle(new CreateSignalCommand(2, 2, "Incidencia dos", "Dos", [2], [22], true, Guid.NewGuid(), incidencia.Id, "tag2"), CancellationToken.None);

        var search = new SearchSignalsQueryHandler(signalRepo);
        var result = await search.Handle(new SearchSignalsQuery(null, null, null, incidencia.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(incidencia.Id, result.Single().Tipo);
        Assert.Equal("Incidencia dos", result.Single().Titulo);
    }

    [Fact]
    public async Task UpdateSignal_ShouldRefreshAuditFields()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var createHandler = new CreateSignalCommandHandler(signalRepo, typeRepo);
        var ownerId = Guid.NewGuid();
        var signalId = await createHandler.Handle(new CreateSignalCommand(1, 1, "Titulo original", "Desc", [5], [6], true, ownerId, aviso.Id, "t"), CancellationToken.None);
        var original = await signalRepo.GetByIdAsync(signalId, CancellationToken.None);

        await Task.Delay(5);
        var updateUser = ownerId;
        var updateHandler = new UpdateSignalCommandHandler(signalRepo);
        await updateHandler.Handle(new UpdateSignalCommand(signalId, "Titulo actualizado", "Desc2", false, updateUser), CancellationToken.None);

        var updated = await signalRepo.GetByIdAsync(signalId, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.True(updated!.FechaModificacion >= original!.FechaModificacion);
        Assert.Equal(updateUser, updated.UserIdModificacion);
        Assert.Equal("Titulo actualizado", updated.Titulo);
        Assert.False(updated.Activo);
        Assert.Equal([5], updated.Foto1);
        Assert.Equal([6], updated.Foto2);
        Assert.Equal(aviso.Id, updated.Tipo);
        Assert.Equal("t", updated.Tags);
    }

    [Fact]
    public async Task UpdateSignal_ShouldThrowForbidden_WhenUserIsNotOwner()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);

        var ownerId = Guid.NewGuid();
        var signalId = await new CreateSignalCommandHandler(signalRepo, typeRepo)
            .Handle(new CreateSignalCommand(1, 1, "Titulo original", "Desc", [5], [6], true, ownerId, aviso.Id, "t"), CancellationToken.None);

        var updateHandler = new UpdateSignalCommandHandler(signalRepo);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            updateHandler.Handle(new UpdateSignalCommand(signalId, "Titulo actualizado", "Desc2", false, Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task GetSignalImages_ShouldReturnBothPhotos()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var createHandler = new CreateSignalCommandHandler(signalRepo, typeRepo);
        var signalId = await createHandler.Handle(new CreateSignalCommand(1, 1, "Titulo imagenes", "Desc", [9], [10], true, Guid.NewGuid(), aviso.Id, "t"), CancellationToken.None);

        var queryHandler = new GetSignalImagesQueryHandler(signalRepo);
        var result = await queryHandler.Handle(new GetSignalImagesQuery(signalId), CancellationToken.None);

        Assert.Equal(signalId, result.SignalId);
        Assert.Equal([9], result.Foto1);
        Assert.Equal([10], result.Foto2);
    }

    [Fact]
    public async Task GetSignalImages_ShouldReturnEmptySecondPhoto_WhenCreatedWithoutIt()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var createHandler = new CreateSignalCommandHandler(signalRepo, typeRepo);
        var signalId = await createHandler.Handle(new CreateSignalCommand(1, 1, "Titulo imagen unica", "Desc", [9], null, true, Guid.NewGuid(), aviso.Id, "t"), CancellationToken.None);

        var queryHandler = new GetSignalImagesQueryHandler(signalRepo);
        var result = await queryHandler.Handle(new GetSignalImagesQuery(signalId), CancellationToken.None);

        Assert.Equal(signalId, result.SignalId);
        Assert.Equal([9], result.Foto1);
        Assert.Empty(result.Foto2);
    }

    [Fact]
    public async Task CreateSignalComment_ShouldPersistAndReturnId()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var signalId = await new CreateSignalCommandHandler(signalRepo, typeRepo)
            .Handle(new CreateSignalCommand(1, 1, "Titulo comentario", "Desc", [1], [2], true, Guid.NewGuid(), aviso.Id, "tag"), CancellationToken.None);

        var commentUserId = Guid.NewGuid();
        var handler = new CreateSignalCommentCommandHandler(signalRepo);
        var commentId = await handler.Handle(new CreateSignalCommentCommand(signalId, commentUserId, "Hay paso estrecho."), CancellationToken.None);

        var comments = await signalRepo.GetCommentsBySignalIdAsync(signalId, CancellationToken.None);
        var comment = Assert.Single(comments);
        Assert.Equal(commentId, comment.Id);
        Assert.Equal(commentUserId, comment.UserId);
        Assert.Equal("Hay paso estrecho.", comment.Texto);
    }

    [Fact]
    public async Task CreateSignalComment_ShouldThrowWhenSignalMissing()
    {
        var signalRepo = new InMemorySignalRepository();
        var handler = new CreateSignalCommentCommandHandler(signalRepo);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CreateSignalCommentCommand(Guid.NewGuid(), Guid.NewGuid(), "Comentario"), CancellationToken.None));
    }

    [Fact]
    public void CreateSignalCommentValidator_ShouldRejectEmptyText()
    {
        var validator = new CreateSignalCommentCommandValidator();

        var result = validator.Validate(new CreateSignalCommentCommand(Guid.NewGuid(), Guid.NewGuid(), " "));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Texto");
    }

    [Fact]
    public async Task GetSignalComments_ShouldReturnChronologicalCollection()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var signalId = await new CreateSignalCommandHandler(signalRepo, typeRepo)
            .Handle(new CreateSignalCommand(1, 1, "Titulo historial", "Desc", [1], [2], true, Guid.NewGuid(), aviso.Id, "tag"), CancellationToken.None);

        var signal = await signalRepo.GetByIdAsync(signalId, CancellationToken.None);
        Assert.NotNull(signal);
        signal!.AnadirComentario("Primero", Guid.NewGuid(), new DateTime(2026, 5, 31, 10, 0, 0, DateTimeKind.Utc));
        signal.AnadirComentario("Segundo", Guid.NewGuid(), new DateTime(2026, 5, 31, 11, 0, 0, DateTimeKind.Utc));

        var handler = new GetSignalCommentsQueryHandler(signalRepo);
        var result = await handler.Handle(new GetSignalCommentsQuery(signalId), CancellationToken.None);
        var items = result.ToArray();

        Assert.Equal(2, items.Length);
        Assert.Equal("Primero", items[0].Texto);
        Assert.Equal("Segundo", items[1].Texto);
    }

    [Fact]
    public async Task GetSignalById_ShouldReturnTitle()
    {
        var signalRepo = new InMemorySignalRepository();
        var typeRepo = new InMemorySignalTypeRepository();
        var aviso = SignalType.Crear("Aviso", "i");
        await typeRepo.AddAsync(aviso, CancellationToken.None);
        var signalId = await new CreateSignalCommandHandler(signalRepo, typeRepo)
            .Handle(new CreateSignalCommand(1, 1, "Titulo detalle", "Descripcion detalle", [1], [2], true, Guid.NewGuid(), aviso.Id, "tag"), CancellationToken.None);

        var handler = new Features.TrailSignals.Signals.GetSignalById.GetSignalByIdQueryHandler(signalRepo);
        var result = await handler.Handle(new Features.TrailSignals.Signals.GetSignalById.GetSignalByIdQuery(signalId), CancellationToken.None);

        Assert.Equal("Titulo detalle", result.Titulo);
        Assert.Equal("Descripcion detalle", result.Descripcion);
    }

    private sealed class InMemorySignalTypeRepository : ISignalTypeRepository
    {
        private static readonly FieldInfo IdField = typeof(SignalType).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private readonly Dictionary<int, SignalType> _items = [];
        private int _nextId = 1;

        public Task AddAsync(SignalType signalType, CancellationToken cancellationToken)
        {
            if (signalType.Id == 0)
            {
                IdField.SetValue(signalType, _nextId++);
            }

            _items[signalType.Id] = signalType;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<SignalType>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyCollection<SignalType>)_items.Values.OrderBy(x => x.Id).ToArray());

        public Task<SignalType?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.TryGetValue(id, out var signalType) ? signalType : null);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.ContainsKey(id));

        public void Remove(SignalType signalType)
            => _items.Remove(signalType.Id);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private sealed class InMemorySignalRepository : ISignalRepository
    {
        private readonly List<Signal> _items = [];

        public Task AddAsync(Signal signal, CancellationToken cancellationToken)
        {
            _items.Add(signal);
            return Task.CompletedTask;
        }

        public Task<Signal?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(_items.SingleOrDefault(x => x.Id == id));

        public Task<IReadOnlyCollection<SignalComment>> GetCommentsBySignalIdAsync(Guid signalId, CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyCollection<SignalComment>)_items
                .SingleOrDefault(x => x.Id == signalId)?
                .Comentarios
                .OrderBy(x => x.FechaComentario)
                .ThenBy(x => x.Id)
                .ToArray() ?? []);

        public Task<IReadOnlyCollection<Signal>> SearchAsync(string? tags, bool? activo, string? descripcion, int? tipo, CancellationToken cancellationToken)
        {
            IEnumerable<Signal> query = _items;
            if (!string.IsNullOrWhiteSpace(tags)) query = query.Where(x => x.Tags.Contains(tags, StringComparison.OrdinalIgnoreCase));
            if (activo.HasValue) query = query.Where(x => x.Activo == activo.Value);
            if (!string.IsNullOrWhiteSpace(descripcion)) query = query.Where(x => x.Descripcion.Contains(descripcion, StringComparison.OrdinalIgnoreCase));
            if (tipo.HasValue) query = query.Where(x => x.Tipo == tipo.Value);
            return Task.FromResult((IReadOnlyCollection<Signal>)query.ToArray());
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
