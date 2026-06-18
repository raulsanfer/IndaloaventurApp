using System.Reflection;
using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.CreateCargo;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.DeleteCargo;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.GetAllCargos;
using IndaloAventurApi.Application.Features.ClubPositions.Cargos.UpdateCargo;
using IndaloAventurApi.Domain.ClubPositions;

namespace IndaloAventurApi.Application.Tests.ClubPositions;

public sealed class CargosHandlersTests
{
    [Fact]
    public async Task CreateCargo_ShouldPersistAndReturnId()
    {
        var repo = new InMemoryCargoRepository();
        var handler = new CreateCargoCommandHandler(repo);

        var id = await handler.Handle(new CreateCargoCommand("Presidente"), CancellationToken.None);

        Assert.Equal(1, id);
        Assert.NotNull(await repo.GetByIdAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCargo_ShouldThrowWhenMissing()
    {
        var repo = new InMemoryCargoRepository();
        var handler = new UpdateCargoCommandHandler(repo);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new UpdateCargoCommand(77, "Nuevo"), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCargo_ShouldThrowWhenAssigned()
    {
        var repo = new InMemoryCargoRepository { IsAssigned = true };
        var cargo = Cargo.Crear("Secretario");
        await repo.AddAsync(cargo, CancellationToken.None);
        var handler = new DeleteCargoCommandHandler(repo);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new DeleteCargoCommand(cargo.Id), CancellationToken.None));
    }

    [Fact]
    public async Task GetAllCargos_ShouldReturnDtos()
    {
        var repo = new InMemoryCargoRepository();
        var presidente = Cargo.Crear("Presidente");
        await repo.AddAsync(presidente, CancellationToken.None);
        var tesorero = Cargo.Crear("Tesorero");
        await repo.AddAsync(tesorero, CancellationToken.None);
        var handler = new GetAllCargosQueryHandler(repo);

        var result = await handler.Handle(new GetAllCargosQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == presidente.Id && x.Descripcion == "Presidente");
    }

    private sealed class InMemoryCargoRepository : ICargoRepository
    {
        private static readonly FieldInfo IdField = typeof(Cargo).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private readonly Dictionary<int, Cargo> _items = [];
        private int _nextId = 1;

        public bool IsAssigned { get; set; }

        public Task AddAsync(Cargo cargo, CancellationToken cancellationToken)
        {
            if (cargo.Id == 0)
            {
                IdField.SetValue(cargo, _nextId++);
            }

            _items[cargo.Id] = cargo;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Cargo>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyCollection<Cargo>)_items.Values.OrderBy(x => x.Id).ToArray());

        public Task<Cargo?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.TryGetValue(id, out var cargo) ? cargo : null);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.ContainsKey(id));

        public Task<bool> IsAssignedToAnyUserAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(IsAssigned);

        public void Remove(Cargo cargo)
            => _items.Remove(cargo.Id);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
