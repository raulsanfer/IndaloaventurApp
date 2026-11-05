using IndaloAventurApi.Domain.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

public interface IReservaMaterialRepository
{
    Task<ReservaMaterial?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ReservaMaterial?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken);
    Task<int> GetCantidadComprometidaAsync(int inventarioMaterialId, Guid? excludingReservaId, CancellationToken cancellationToken);
    Task AddAsync(ReservaMaterial reserva, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<T> ExecuteSerializableAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken);
}
