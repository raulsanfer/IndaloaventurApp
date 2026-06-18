using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Domain.TrailSignals;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.TrailSignals;

public sealed class SignalTypeRepository(ApplicationDbContext dbContext) : ISignalTypeRepository
{
    public Task AddAsync(SignalType signalType, CancellationToken cancellationToken)
        => dbContext.SignalTypes.AddAsync(signalType, cancellationToken).AsTask();

    public async Task<IReadOnlyCollection<SignalType>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.SignalTypes
            .OrderBy(x => x.Id)
            .ToArrayAsync(cancellationToken);

    public Task<SignalType?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => dbContext.SignalTypes.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        => dbContext.SignalTypes.AnyAsync(x => x.Id == id, cancellationToken);

    public void Remove(SignalType signalType)
        => dbContext.SignalTypes.Remove(signalType);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
