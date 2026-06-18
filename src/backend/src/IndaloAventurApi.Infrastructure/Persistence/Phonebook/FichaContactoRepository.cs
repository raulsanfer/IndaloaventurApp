using IndaloAventurApi.Application.Abstractions.Phonebook;
using IndaloAventurApi.Domain.FichasContacto;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.Phonebook;

public sealed class FichaContactoRepository(ApplicationDbContext dbContext) : IFichaContactoRepository
{
    public async Task AddAsync(FichaContacto fichaContacto, CancellationToken cancellationToken)
    {
        await dbContext.FichasContacto.AddAsync(fichaContacto, cancellationToken);
    }

    public Task<FichaContacto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.FichasContacto.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<FichaContacto>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.FichasContacto
            .OrderBy(x => x.Nombre.Value)
            .ThenBy(x => x.FechaAlta)
            .ToListAsync(cancellationToken);
    }

    public void Remove(FichaContacto fichaContacto)
    {
        dbContext.FichasContacto.Remove(fichaContacto);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}