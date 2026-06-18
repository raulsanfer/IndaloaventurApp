using IndaloAventurApi.Domain.FichasSocio;

namespace IndaloAventurApi.Application.Abstractions.FichasSocio;

public interface IFichaSocioRepository
{
    Task<FichaSocio?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(FichaSocio fichaSocio, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
