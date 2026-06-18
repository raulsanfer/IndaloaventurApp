using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Domain.TrailSignals;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.TrailSignals;

public sealed class SignalRepository(ApplicationDbContext dbContext) : ISignalRepository
{
    public Task AddAsync(Signal signal, CancellationToken cancellationToken)
        => dbContext.Signals.AddAsync(signal, cancellationToken).AsTask();

    public Task<Signal?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.Signals.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<SignalComment>> GetCommentsBySignalIdAsync(Guid signalId, CancellationToken cancellationToken)
        => await dbContext.SignalComments
            .AsNoTracking()
            .Where(x => x.SignalId == signalId)
            .OrderBy(x => x.FechaComentario)
            .ThenBy(x => x.Id)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Signal>> SearchAsync(string? tags, bool? activo, string? descripcion, int? tipo, CancellationToken cancellationToken)
    {
        IQueryable<Signal> query = dbContext.Signals.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tags))
        {
            query = query.Where(x => x.Tags.Contains(tags));
        }
        if (activo.HasValue)
        {
            query = query.Where(x => x.Activo == activo.Value);
        }
        if (!string.IsNullOrWhiteSpace(descripcion))
        {
            query = query.Where(x => x.Descripcion.Contains(descripcion));
        }
        if (tipo.HasValue)
        {
            query = query.Where(x => x.Tipo == tipo.Value);
        }

        return await query
            .OrderByDescending(x => x.FechaModificacion)
            .ToArrayAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
