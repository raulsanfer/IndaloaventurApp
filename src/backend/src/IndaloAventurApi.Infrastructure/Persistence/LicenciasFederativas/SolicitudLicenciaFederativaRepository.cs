using System.Data;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;

public sealed class SolicitudLicenciaFederativaRepository(ApplicationDbContext dbContext) : ISolicitudLicenciaFederativaRepository
{
    public Task<SolicitudLicenciaFederativa?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.SolicitudesLicenciasFederativas
            .Include(x => x.TarifaLicenciaFederativa)
            .ThenInclude(x => x!.TipoLicencia)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAndTemporadaForUpdateAsync(Guid userId, int temporada, CancellationToken cancellationToken)
    {
        IQueryable<SolicitudLicenciaFederativa> query = dbContext.SolicitudesLicenciasFederativas;
        if (dbContext.Database.IsSqlServer())
        {
            if (dbContext.Database.CurrentTransaction is null)
                throw new InvalidOperationException("La consulta de solicitudes para alta requiere una transacción.");
            query = dbContext.SolicitudesLicenciasFederativas.FromSqlInterpolated($"SELECT * FROM SolicitudesLicenciasFederativas WITH (UPDLOCK, HOLDLOCK, INDEX(IX_SolicitudesLicenciasFederativas_UserId_Temporada)) WHERE UserId = {userId} AND Temporada = {temporada}");
        }

        return await query
            .Include(x => x.TarifaLicenciaFederativa)
            .ThenInclude(x => x!.TipoLicencia)
            .Where(x => x.UserId == userId && x.Temporada == temporada)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.SolicitudesLicenciasFederativas
            .Include(x => x.TarifaLicenciaFederativa)
            .ThenInclude(x => x!.TipoLicencia)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Temporada)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SolicitudLicenciaFederativa solicitud, CancellationToken cancellationToken)
    {
        await dbContext.SolicitudesLicenciasFederativas.AddAsync(solicitud, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> ExecuteSerializableAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsRelational())
            return await operation(cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var result = await operation(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result;
    }
}
