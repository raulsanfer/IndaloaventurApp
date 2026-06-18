using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;

public sealed class TarifaLicenciaFederativaRepository(ApplicationDbContext dbContext) : ITarifaLicenciaFederativaRepository
{
    public Task<TarifaLicenciaFederativa?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.TarifasLicenciasFederativas.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListAsync(int? temporada, bool? mediaTemporada, CancellationToken cancellationToken)
    {
        var query = dbContext.TarifasLicenciasFederativas.AsQueryable();

        if (temporada.HasValue)
        {
            query = query.Where(x => x.Temporada == temporada.Value);
        }

        if (mediaTemporada.HasValue)
        {
            query = query.Where(x => x.MediaTemporada == mediaTemporada.Value);
        }

        return await query
            .OrderByDescending(x => x.Temporada)
            .ThenBy(x => x.Licencia)
            .ThenBy(x => x.Categoria)
            .ThenBy(x => x.MediaTemporada)
            .ThenBy(x => x.Territorio)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListByTemporadaAsync(int temporada, CancellationToken cancellationToken)
    {
        return await ListAsync(temporada, null, cancellationToken);
    }

    public async Task AddAsync(TarifaLicenciaFederativa tarifa, CancellationToken cancellationToken)
    {
        await dbContext.TarifasLicenciasFederativas.AddAsync(tarifa, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
