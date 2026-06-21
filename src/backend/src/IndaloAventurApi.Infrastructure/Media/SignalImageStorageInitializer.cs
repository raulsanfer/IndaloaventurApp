using System.Data;
using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.Infrastructure.Media;

public static class SignalImageStorageInitializer
{
    public static async Task InitializeSignalImageStorageAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var storage = scope.ServiceProvider.GetRequiredService<ISignalImageStorage>();

        _ = storage;

        if (!string.Equals(dbContext.Database.ProviderName, "Microsoft.EntityFrameworkCore.SqlServer", StringComparison.Ordinal))
        {
            return;
        }

        var connection = (SqlConnection)dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            var columns = await GetSignalColumnsAsync(connection, cancellationToken);
            var hasLegacyColumns = columns.Contains("Foto1", StringComparer.OrdinalIgnoreCase) &&
                columns.Contains("Foto2", StringComparer.OrdinalIgnoreCase);
            var hasPathColumns = columns.Contains("Foto1Path", StringComparer.OrdinalIgnoreCase) &&
                columns.Contains("Foto2Path", StringComparer.OrdinalIgnoreCase);

            if (!hasLegacyColumns || !hasPathColumns)
            {
                return;
            }

            var rows = await LoadSignalsPendingBackfillAsync(connection, cancellationToken);
            foreach (var row in rows)
            {
                var storedImages = await storage.SaveAsync(row.SignalId, row.Foto1, row.Foto2, cancellationToken);
                await UpdateSignalImagePathsAsync(connection, row.SignalId, storedImages, cancellationToken);
            }

            await DropLegacyColumnsAsync(connection, cancellationToken);
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task<HashSet<string>> GetSignalColumnsAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = 'Signals'
            """;

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(reader.GetString(0));
        }

        return result;
    }

    private static async Task<IReadOnlyCollection<LegacySignalImageRow>> LoadSignalsPendingBackfillAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT [Id], [Foto1], [Foto2], [Foto1Path], [Foto2Path]
            FROM [Signals]
            WHERE ([Foto1Path] IS NULL OR [Foto1Path] = '')
               OR (([Foto2] IS NOT NULL AND DATALENGTH([Foto2]) > 0) AND ([Foto2Path] IS NULL OR [Foto2Path] = ''))
            """;

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var rows = new List<LegacySignalImageRow>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var signalId = reader.GetGuid(0);
            var foto1 = (byte[])reader[1];
            var foto2 = reader.IsDBNull(2) ? [] : (byte[])reader[2];
            rows.Add(new LegacySignalImageRow(signalId, foto1, foto2));
        }

        return rows;
    }

    private static async Task UpdateSignalImagePathsAsync(SqlConnection connection, Guid signalId, StoredSignalImages storedImages, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE [Signals]
            SET [Foto1Path] = @foto1Path,
                [Foto2Path] = @foto2Path
            WHERE [Id] = @id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", signalId);
        command.Parameters.AddWithValue("@foto1Path", storedImages.Foto1Path);
        command.Parameters.AddWithValue("@foto2Path", storedImages.Foto2Path);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task DropLegacyColumnsAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Signals' AND COLUMN_NAME = 'Foto1')
               AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Signals' AND COLUMN_NAME = 'Foto2')
            BEGIN
                ALTER TABLE [Signals] DROP COLUMN [Foto1], [Foto2];
            END
            """;

        using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private sealed record LegacySignalImageRow(Guid SignalId, byte[] Foto1, byte[] Foto2);
}
