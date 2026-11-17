using IndaloAventurApi.Domain.LicenciasFederativas;

namespace IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

public interface ISolicitudLicenciaFederativaRepository
{
    Task<SolicitudLicenciaFederativa?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAndTemporadaForUpdateAsync(Guid userId, int temporada, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<T> ExecuteSerializableAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken);
    Task AddAsync(SolicitudLicenciaFederativa solicitud, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
