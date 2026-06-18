using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Domain.ClubPositions;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.ClubPositions;

public sealed class CargoRepository(ApplicationDbContext dbContext) : ICargoRepository
{
    public Task AddAsync(Cargo cargo, CancellationToken cancellationToken)
        => dbContext.Cargos.AddAsync(cargo, cancellationToken).AsTask();

    public async Task<IReadOnlyCollection<Cargo>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.Cargos.OrderBy(x => x.Id).ToArrayAsync(cancellationToken);

    public Task<Cargo?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => dbContext.Cargos.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        => dbContext.Cargos.AnyAsync(x => x.Id == id, cancellationToken);

    public Task<bool> IsAssignedToAnyUserAsync(int id, CancellationToken cancellationToken)
        => Task.FromResult(false);

    public void Remove(Cargo cargo)
        => dbContext.Cargos.Remove(cargo);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
