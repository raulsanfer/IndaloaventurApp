using IndaloAventurApi.Application.Abstractions.Phonebook;
using IndaloAventurApi.Application.Features.AgendaTelefonica.CreateFichaContacto;
using IndaloAventurApi.Application.Features.AgendaTelefonica.DeleteFichaContacto;
using IndaloAventurApi.Application.Features.AgendaTelefonica.GetFichaContactoById;
using IndaloAventurApi.Application.Features.AgendaTelefonica.UpdateFichaContacto;
using IndaloAventurApi.Domain.FichasContacto;

namespace IndaloAventurApi.Application.Tests.AgendaTelefonica;

public sealed class AgendaTelefonicaHandlersTests
{
    [Fact]
    public async Task CreateHandler_ShouldPersistAndReturnId()
    {
        var repository = new InMemoryFichaContactoRepository();
        var handler = new CreateFichaContactoCommandHandler(repository);

        var id = await handler.Handle(
            new CreateFichaContactoCommand("Refugio", "+34 600111222", null, "refugio@club.test", "Calle Refugio 1", null),
            CancellationToken.None);

        var created = await repository.GetByIdAsync(id, CancellationToken.None);
        Assert.NotNull(created);
        Assert.Equal("refugio@club.test", created!.Email?.Value);
        Assert.Equal("Calle Refugio 1", created.Direccion?.Value);
    }

    [Fact]
    public async Task UpdateHandler_ShouldThrowWhenFichaDoesNotExist()
    {
        var repository = new InMemoryFichaContactoRepository();
        var handler = new UpdateFichaContactoCommandHandler(repository);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateFichaContactoCommand(Guid.NewGuid(), "Nombre", "+34 600111222", null, null, null, null),
            CancellationToken.None));
    }

    [Fact]
    public async Task DeleteHandler_ShouldThrowWhenFichaDoesNotExist()
    {
        var repository = new InMemoryFichaContactoRepository();
        var handler = new DeleteFichaContactoCommandHandler(repository);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new DeleteFichaContactoCommand(Guid.NewGuid()),
            CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdHandler_ShouldReturnExistingFicha()
    {
        var repository = new InMemoryFichaContactoRepository();
        var ficha = FichaContacto.Crear("Refugio", "+34 600111222", null, "refugio@club.test", "Calle Refugio 1", "Observaciones");
        await repository.AddAsync(ficha, CancellationToken.None);

        var handler = new GetFichaContactoByIdQueryHandler(repository);
        var result = await handler.Handle(new GetFichaContactoByIdQuery(ficha.Id), CancellationToken.None);

        Assert.Equal(ficha.Id, result.Id);
        Assert.Equal("Refugio", result.Nombre);
        Assert.Equal("refugio@club.test", result.Email);
        Assert.Equal("Calle Refugio 1", result.Direccion);
    }

    private sealed class InMemoryFichaContactoRepository : IFichaContactoRepository
    {
        private readonly List<FichaContacto> _items = [];

        public Task AddAsync(FichaContacto fichaContacto, CancellationToken cancellationToken)
        {
            _items.Add(fichaContacto);
            return Task.CompletedTask;
        }

        public Task<FichaContacto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_items.SingleOrDefault(x => x.Id == id));
        }

        public Task<IReadOnlyCollection<FichaContacto>> ListAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IReadOnlyCollection<FichaContacto>)_items.ToArray());
        }

        public void Remove(FichaContacto fichaContacto)
        {
            _items.Remove(fichaContacto);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
