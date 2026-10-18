using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class InventarioCategoriaRepository(ApplicationDbContext dbContext) : IInventarioCategoriaRepository
{
    public Task AddAsync(InventarioCategoria categoria, CancellationToken cancellationToken)
        => dbContext.InventarioCategorias.AddAsync(categoria, cancellationToken).AsTask();

    public Task<InventarioCategoria?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => dbContext.InventarioCategorias.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsWithNormalizedNameAsync(string nombreNormalizado, int? categoriaPadreId, int? excludingId, CancellationToken cancellationToken)
        => dbContext.InventarioCategorias.AnyAsync(
            x => x.NombreNormalizado == nombreNormalizado
                && x.CategoriaPadreId == categoriaPadreId
                && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);

    public Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken)
        => dbContext.InventarioCategorias.AnyAsync(x => x.CategoriaPadreId == id, cancellationToken);

    public void Remove(InventarioCategoria categoria)
        => dbContext.InventarioCategorias.Remove(categoria);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException("No se puede guardar la categoria de inventario porque incumple una restriccion de integridad.", exception);
        }
    }
}
