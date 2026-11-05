namespace IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

public interface ISocioReservaReader
{
    Task<SocioReservaDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
