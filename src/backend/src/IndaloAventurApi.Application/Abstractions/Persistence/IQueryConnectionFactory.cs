using System.Data;

namespace IndaloAventurApi.Application.Abstractions.Persistence;

public interface IQueryConnectionFactory
{
    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken);
}
