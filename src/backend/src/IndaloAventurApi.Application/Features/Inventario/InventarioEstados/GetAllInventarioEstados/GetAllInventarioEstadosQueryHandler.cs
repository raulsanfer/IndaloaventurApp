using Dapper;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Persistence;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.GetAllInventarioEstados;

public sealed class GetAllInventarioEstadosQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetAllInventarioEstadosQuery, IReadOnlyCollection<InventarioEstadoDto>>
{
    public async Task<IReadOnlyCollection<InventarioEstadoDto>> Handle(GetAllInventarioEstadosQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Nombre
            FROM InventarioEstados
            ORDER BY Nombre ASC, Id ASC
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<InventarioEstadoRow>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.Select(x => new InventarioEstadoDto(x.Id, x.Nombre)).ToArray();
    }

    private sealed class InventarioEstadoRow
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
