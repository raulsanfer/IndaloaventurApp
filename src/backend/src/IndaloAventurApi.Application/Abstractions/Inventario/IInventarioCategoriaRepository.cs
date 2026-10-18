using IndaloAventurApi.Domain.Inventario;

namespace IndaloAventurApi.Application.Abstractions.Inventario;

public interface IInventarioCategoriaRepository
{
    Task AddAsync(InventarioCategoria categoria, CancellationToken cancellationToken);
    Task<InventarioCategoria?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? categoriaPadreId, int? excludingId, CancellationToken cancellationToken);
    Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken);
    void Remove(InventarioCategoria categoria);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
