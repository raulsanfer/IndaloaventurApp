using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class InventarioMaterialRepository(ApplicationDbContext dbContext) : IInventarioMaterialRepository
{
    public Task AddAsync(InventarioMaterial material, CancellationToken cancellationToken)
        => dbContext.InventarioMateriales.AddAsync(material, cancellationToken).AsTask();

    public Task<InventarioMaterial?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => dbContext.InventarioMateriales.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<InventarioMaterial?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsSqlServer())
        {
            return GetByIdAsync(id, cancellationToken);
        }

        return dbContext.InventarioMateriales
            .FromSqlInterpolated($"SELECT * FROM InventarioMateriales WITH (UPDLOCK, HOLDLOCK) WHERE Id = {id}")
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsWithNormalizedSerialAsync(string normalizedSerial, int? excludingId, CancellationToken cancellationToken)
        => dbContext.InventarioMateriales.AnyAsync(
            x => x.NumeroSerieNormalizado == normalizedSerial && (!excludingId.HasValue || x.Id != excludingId),
            cancellationToken);

    public void Remove(InventarioMaterial material) => dbContext.InventarioMateriales.Remove(material);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException("No se puede guardar el material de inventario porque incumple una restriccion de integridad.", exception);
        }
    }
}
