using IndaloAventurApi.Domain.FichasContacto;

namespace IndaloAventurApi.Application.Abstractions.Phonebook;

public interface IFichaContactoRepository
{
    Task AddAsync(FichaContacto fichaContacto, CancellationToken cancellationToken);
    Task<FichaContacto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<FichaContacto>> ListAsync(CancellationToken cancellationToken);
    void Remove(FichaContacto fichaContacto);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}