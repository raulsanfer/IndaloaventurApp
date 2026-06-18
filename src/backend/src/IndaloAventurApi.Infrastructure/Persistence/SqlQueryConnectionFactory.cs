using IndaloAventurApi.Application.Abstractions.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace IndaloAventurApi.Infrastructure.Persistence;

public sealed class SqlQueryConnectionFactory(IConfiguration configuration) : IQueryConnectionFactory
{
    public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connectionString = configuration.GetConnectionString("api_ContextConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No database connection string configured. Expected 'api_ContextConnection' or 'DefaultConnection'.");

        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
