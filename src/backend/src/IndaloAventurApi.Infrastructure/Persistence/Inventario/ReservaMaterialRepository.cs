using System.Data;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class ReservaMaterialRepository(ApplicationDbContext dbContext) : IReservaMaterialRepository
{
    public Task<ReservaMaterial?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.ReservasMaterial.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<ReservaMaterial?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsSqlServer())
        {
            return GetByIdAsync(id, cancellationToken);
        }

        return dbContext.ReservasMaterial
            .FromSqlInterpolated($"SELECT * FROM ReservasMaterial WITH (UPDLOCK, HOLDLOCK) WHERE Id = {id}")
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetCantidadComprometidaAsync(int inventarioMaterialId, Guid? excludingReservaId, CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsSqlServer())
        {
            return await dbContext.ReservasMaterial
                .Where(x => x.InventarioMaterialId == inventarioMaterialId
                            && x.Id != excludingReservaId
                            && (x.Estado == EstadoReservaMaterial.Solicitada || x.Estado == EstadoReservaMaterial.Retirada))
                .SumAsync(x => (int?)x.CantidadSolicitada, cancellationToken) ?? 0;
        }

        var connection = dbContext.Database.GetDbConnection();
        var openedHere = connection.State != ConnectionState.Open;
        if (openedHere)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.Transaction = dbContext.Database.CurrentTransaction?.GetDbTransaction();
            command.CommandText = """
                SELECT COALESCE(SUM(r.CantidadSolicitada), 0)
                FROM InventarioMateriales m WITH (UPDLOCK, HOLDLOCK)
                LEFT JOIN ReservasMaterial r WITH (UPDLOCK, HOLDLOCK)
                    ON r.InventarioMaterialId = m.Id
                    AND r.Estado IN (@solicitada, @retirada)
                    AND (@excludingReservaId IS NULL OR r.Id <> @excludingReservaId)
                WHERE m.Id = @materialId
                GROUP BY m.Id;
                """;
            AddParameter(command, "@solicitada", (int)EstadoReservaMaterial.Solicitada, DbType.Int32);
            AddParameter(command, "@retirada", (int)EstadoReservaMaterial.Retirada, DbType.Int32);
            AddParameter(command, "@excludingReservaId", excludingReservaId, DbType.Guid);
            AddParameter(command, "@materialId", inventarioMaterialId, DbType.Int32);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is null or DBNull ? 0 : Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture);
        }
        finally
        {
            if (openedHere)
            {
                await connection.CloseAsync();
            }
        }
    }

    public Task AddAsync(ReservaMaterial reserva, CancellationToken cancellationToken)
        => dbContext.ReservasMaterial.AddAsync(reserva, cancellationToken).AsTask();

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new InvalidOperationException("La reserva se ha modificado de forma concurrente. Vuelve a intentarlo.", exception);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException("No se puede guardar la reserva de material porque incumple una restriccion de integridad.", exception);
        }
    }

    public async Task<T> ExecuteSerializableAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        if (dbContext.Database.CurrentTransaction is not null)
        {
            return await operation(cancellationToken);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var result = await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void AddParameter(IDbCommand command, string name, object? value, DbType dbType)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = dbType;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
