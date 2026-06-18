using IndaloAventurApi.Domain.TrailSignals;

namespace IndaloAventurApi.Application.Abstractions.TrailSignals;

public interface ISignalTypeRepository
{
    Task AddAsync(SignalType signalType, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SignalType>> GetAllAsync(CancellationToken cancellationToken);
    Task<SignalType?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    void Remove(SignalType signalType);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
