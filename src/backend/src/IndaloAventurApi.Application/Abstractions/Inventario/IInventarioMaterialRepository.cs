using IndaloAventurApi.Domain.Inventario;

namespace IndaloAventurApi.Application.Abstractions.Inventario;

public interface IInventarioMaterialRepository
{
    Task AddAsync(InventarioMaterial material, CancellationToken cancellationToken);
    Task<InventarioMaterial?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<InventarioMaterial?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsWithNormalizedSerialAsync(string normalizedSerial, int? excludingId, CancellationToken cancellationToken);
    void Remove(InventarioMaterial material);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
