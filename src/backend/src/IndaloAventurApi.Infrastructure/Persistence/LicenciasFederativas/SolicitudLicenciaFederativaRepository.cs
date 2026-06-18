using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;

public sealed class SolicitudLicenciaFederativaRepository(ApplicationDbContext dbContext) : ISolicitudLicenciaFederativaRepository
{
    public Task<SolicitudLicenciaFederativa?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.SolicitudesLicenciasFederativas
            .Include(x => x.TarifaLicenciaFederativa)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<SolicitudLicenciaFederativa?> GetByUserIdAndTemporadaAsync(Guid userId, int temporada, CancellationToken cancellationToken)
    {
        return dbContext.SolicitudesLicenciasFederativas
            .Include(x => x.TarifaLicenciaFederativa)
            .SingleOrDefaultAsync(x => x.UserId == userId && x.Temporada == temporada, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.SolicitudesLicenciasFederativas
            .Include(x => x.TarifaLicenciaFederativa)
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
}
