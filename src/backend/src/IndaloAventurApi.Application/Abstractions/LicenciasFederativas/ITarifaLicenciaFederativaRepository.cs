using IndaloAventurApi.Domain.LicenciasFederativas;

namespace IndaloAventurApi.Application.Abstractions.LicenciasFederativas;

public interface ITarifaLicenciaFederativaRepository
{
    Task<TarifaLicenciaFederativa?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListAsync(int? temporada, bool? mediaTemporada, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListByTemporadaAsync(int temporada, CancellationToken cancellationToken);
    Task AddAsync(TarifaLicenciaFederativa tarifa, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
