using System.Reflection;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.CreateInventarioEstado;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.DeleteInventarioEstado;
using IndaloAventurApi.Application.Features.Inventario.InventarioEstados.UpdateInventarioEstado;
using IndaloAventurApi.Domain.Inventario;

namespace IndaloAventurApi.Application.Tests.Inventario;

public sealed class InventarioEstadoHandlersTests
{
    [Fact]
    public async Task Create_NormalizaElNombreYAsignaIdentificador()
    {
        var repository = new InMemoryInventarioEstadoRepository();
        var handler = new CreateInventarioEstadoCommandHandler(repository);

        var id = await handler.Handle(new CreateInventarioEstadoCommand(" Disponible "), CancellationToken.None);
        var estado = await repository.GetByIdAsync(id, CancellationToken.None);

        Assert.NotNull(estado);
        Assert.Equal("Disponible", estado.Nombre);
    }

    [Fact]
    public async Task Create_NombreDuplicadoNormalizado_RechazaElConflicto()
    {
        var repository = new InMemoryInventarioEstadoRepository();
        var handler = new CreateInventarioEstadoCommandHandler(repository);
        await handler.Handle(new CreateInventarioEstadoCommand("Disponible"), CancellationToken.None);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new CreateInventarioEstadoCommand(" disponible "),
            CancellationToken.None));

        Assert.Equal("Ya existe un estado de inventario con ese nombre.", exception.Message);
    }

    [Fact]
    public async Task Update_EstadoInexistente_DevuelveNoEncontrado()
    {
        var repository = new InMemoryInventarioEstadoRepository();
        var handler = new UpdateInventarioEstadoCommandHandler(repository);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateInventarioEstadoCommand(999, "Disponible"),
            CancellationToken.None));

        Assert.Equal("El estado de inventario no existe.", exception.Message);
    }

    [Fact]
    public async Task Delete_EstadoExistente_EliminaElRegistro()
    {
        var repository = new InMemoryInventarioEstadoRepository();
        var estado = InventarioEstado.Crear("Disponible");
        await repository.AddAsync(estado, CancellationToken.None);
        var handler = new DeleteInventarioEstadoCommandHandler(repository);

        await handler.Handle(new DeleteInventarioEstadoCommand(estado.Id), CancellationToken.None);

        Assert.Null(await repository.GetByIdAsync(estado.Id, CancellationToken.None));
    }

    private sealed class InMemoryInventarioEstadoRepository : IInventarioEstadoRepository
    {
        private static readonly FieldInfo IdField = typeof(InventarioEstado).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private readonly Dictionary<int, InventarioEstado> _items = [];
        private int _nextId = 1;

        public Task AddAsync(InventarioEstado estado, CancellationToken cancellationToken)
        {
            if (estado.Id == 0)
            {
                IdField.SetValue(estado, _nextId++);
            }

            _items[estado.Id] = estado;
            return Task.CompletedTask;
        }

        public Task<InventarioEstado?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.TryGetValue(id, out var estado) ? estado : null);

        public Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? excludingId, CancellationToken cancellationToken)
            => Task.FromResult(_items.Values.Any(x => x.NombreNormalizado == nombreNormalizado && x.Id != excludingId));

        public void Remove(InventarioEstado estado) => _items.Remove(estado.Id);

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
