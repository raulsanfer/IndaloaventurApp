using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class InventarioEstadoRepository(ApplicationDbContext dbContext) : IInventarioEstadoRepository
{
    public Task AddAsync(InventarioEstado estado, CancellationToken cancellationToken)
        => dbContext.InventarioEstados.AddAsync(estado, cancellationToken).AsTask();

    public Task<InventarioEstado?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => dbContext.InventarioEstados.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? excludingId, CancellationToken cancellationToken)
        => dbContext.InventarioEstados.AnyAsync(
            x => x.NombreNormalizado == nombreNormalizado && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);

    public void Remove(InventarioEstado estado)
        => dbContext.InventarioEstados.Remove(estado);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException("No se puede guardar el estado de inventario porque incumple una restriccion de integridad.", exception);
        }
    }
}
