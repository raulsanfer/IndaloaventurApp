using IndaloAventurApi.Domain.Inventario;

namespace IndaloAventurApi.Application.Abstractions.Inventario;

public interface IInventarioEstadoRepository
{
    Task AddAsync(InventarioEstado estado, CancellationToken cancellationToken);
    Task<InventarioEstado?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? excludingId, CancellationToken cancellationToken);
    void Remove(InventarioEstado estado);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
