using IndaloAventurApi.Domain.TrailSignals;

namespace IndaloAventurApi.Application.Abstractions.TrailSignals;

public interface ISignalRepository
{
    Task AddAsync(Signal signal, CancellationToken cancellationToken);
    Task<Signal?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SignalComment>> GetCommentsBySignalIdAsync(Guid signalId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Signal>> SearchAsync(string? tags, bool? activo, string? descripcion, int? tipo, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
