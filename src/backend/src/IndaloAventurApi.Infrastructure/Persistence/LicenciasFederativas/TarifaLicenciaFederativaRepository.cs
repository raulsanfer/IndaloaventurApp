using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;

public sealed class TarifaLicenciaFederativaRepository(ApplicationDbContext dbContext) : ITarifaLicenciaFederativaRepository
{
    public Task<TarifaLicenciaFederativa?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.TarifasLicenciasFederativas.Include(x => x.TipoLicencia).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListAsync(int? temporada, int? tipoLicenciaId, CancellationToken cancellationToken)
    {
        var query = dbContext.TarifasLicenciasFederativas.Include(x => x.TipoLicencia).AsQueryable();

        if (temporada.HasValue)
        {
            query = query.Where(x => x.Temporada == temporada.Value);
        }

        if (tipoLicenciaId.HasValue)
        {
            query = query.Where(x => x.TipoLicenciaId == tipoLicenciaId.Value);
        }

        return await query
            .OrderByDescending(x => x.Temporada)
            .ThenBy(x => x.TipoLicenciaId)
            .ThenBy(x => x.Licencia)
            .ThenBy(x => x.Categoria)
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
