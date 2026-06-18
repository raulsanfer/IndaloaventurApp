using IndaloAventurApi.Domain.ClubPositions;

namespace IndaloAventurApi.Application.Abstractions.ClubPositions;

public interface ICargoRepository
{
    Task AddAsync(Cargo cargo, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Cargo>> GetAllAsync(CancellationToken cancellationToken);
    Task<Cargo?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<bool> IsAssignedToAnyUserAsync(int id, CancellationToken cancellationToken);
    void Remove(Cargo cargo);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
