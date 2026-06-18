using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Domain.FichasSocio;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.FichasSocio;

public sealed class FichaSocioRepository(ApplicationDbContext dbContext) : IFichaSocioRepository
{
    public Task<FichaSocio?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.FichasSocio.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(FichaSocio fichaSocio, CancellationToken cancellationToken)
    {
        await dbContext.FichasSocio.AddAsync(fichaSocio, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
