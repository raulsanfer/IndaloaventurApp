using System.Reflection;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.CreateInventarioCategoria;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.DeleteInventarioCategoria;
using IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.UpdateInventarioCategoria;
using IndaloAventurApi.Domain.Inventario;

namespace IndaloAventurApi.Application.Tests.Inventario;

public sealed class InventarioCategoriaHandlersTests
{
    [Fact]
    public async Task Create_SubcategoriaDeSubcategoria_RechazaTercerNivel()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var raiz = await CrearAsync(repository, "Material", null);
        var hija = await CrearAsync(repository, "Cascos", raiz.Id);
        var handler = new CreateInventarioCategoriaCommandHandler(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new CreateInventarioCategoriaCommand("Integral", hija.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Update_CategoriaConHijos_NoPermiteMoverlaBajoOtraCategoria()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var raiz = await CrearAsync(repository, "Material", null);
        await CrearAsync(repository, "Cascos", raiz.Id);
        var destino = await CrearAsync(repository, "Ropa", null);
        var handler = new UpdateInventarioCategoriaCommandHandler(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new UpdateInventarioCategoriaCommand(raiz.Id, raiz.Nombre, destino.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Delete_CategoriaConHijos_RechazaBorradoEnCascada()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var raiz = await CrearAsync(repository, "Material", null);
        await CrearAsync(repository, "Cascos", raiz.Id);
        var handler = new DeleteInventarioCategoriaCommandHandler(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new DeleteInventarioCategoriaCommand(raiz.Id), CancellationToken.None));
        Assert.NotNull(await repository.GetByIdAsync(raiz.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Create_CategoriaPadreInexistente_NoPersisteLaCategoria()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var handler = new CreateInventarioCategoriaCommandHandler(repository);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new CreateInventarioCategoriaCommand("Cascos", 999),
            CancellationToken.None));

        Assert.Equal("La categoria padre no existe.", exception.Message);
        Assert.Equal(0, repository.Count);
    }

    [Fact]
    public async Task Create_NombreNormalizadoDuplicado_RechazaElConflicto()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var handler = new CreateInventarioCategoriaCommandHandler(repository);

        await handler.Handle(new CreateInventarioCategoriaCommand(" Material ", null), CancellationToken.None);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new CreateInventarioCategoriaCommand("material", null),
            CancellationToken.None));

        Assert.Equal("Ya existe una categoria con ese nombre en el mismo nivel.", exception.Message);
    }

    [Fact]
    public async Task Update_Autoreferencia_RechazaElConflicto()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var categoria = await CrearAsync(repository, "Material", null);
        var handler = new UpdateInventarioCategoriaCommandHandler(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new UpdateInventarioCategoriaCommand(categoria.Id, categoria.Nombre, categoria.Id),
            CancellationToken.None));

        Assert.Equal("Una categoria no puede ser su propia categoria padre.", exception.Message);
    }

    [Fact]
    public async Task Update_PadreQueEsSubcategoria_RechazaElTercerNivel()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var raiz = await CrearAsync(repository, "Material", null);
        var subcategoria = await CrearAsync(repository, "Cascos", raiz.Id);
        var origen = await CrearAsync(repository, "Ropa", null);
        var handler = new UpdateInventarioCategoriaCommandHandler(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new UpdateInventarioCategoriaCommand(origen.Id, origen.Nombre, subcategoria.Id),
            CancellationToken.None));

        Assert.Equal("Una subcategoria no puede tener subcategorias.", exception.Message);
    }

    [Fact]
    public async Task Delete_CategoriaInexistente_DevuelveNoEncontrado()
    {
        var repository = new InMemoryInventarioCategoriaRepository();
        var handler = new DeleteInventarioCategoriaCommandHandler(repository);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new DeleteInventarioCategoriaCommand(999),
            CancellationToken.None));

        Assert.Equal("La categoria de inventario no existe.", exception.Message);
    }

    private static async Task<InventarioCategoria> CrearAsync(InMemoryInventarioCategoriaRepository repository, string nombre, int? padreId)
    {
        var categoria = InventarioCategoria.Crear(nombre, padreId);
        await repository.AddAsync(categoria, CancellationToken.None);
        return categoria;
    }

    private sealed class InMemoryInventarioCategoriaRepository : IInventarioCategoriaRepository
    {
        private static readonly FieldInfo IdField = typeof(InventarioCategoria).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private readonly Dictionary<int, InventarioCategoria> _items = [];
        private int _nextId = 1;

        public int Count => _items.Count;

        public Task AddAsync(InventarioCategoria categoria, CancellationToken cancellationToken)
        {
            if (categoria.Id == 0) IdField.SetValue(categoria, _nextId++);
            _items[categoria.Id] = categoria;
            return Task.CompletedTask;
        }

        public Task<InventarioCategoria?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.TryGetValue(id, out var categoria) ? categoria : null);

        public Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? categoriaPadreId, int? excludingId, CancellationToken cancellationToken)
            => Task.FromResult(_items.Values.Any(x => x.NombreNormalizado == nombreNormalizado && x.CategoriaPadreId == categoriaPadreId && x.Id != excludingId));

        public Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_items.Values.Any(x => x.CategoriaPadreId == id));

        public void Remove(InventarioCategoria categoria) => _items.Remove(categoria.Id);
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
