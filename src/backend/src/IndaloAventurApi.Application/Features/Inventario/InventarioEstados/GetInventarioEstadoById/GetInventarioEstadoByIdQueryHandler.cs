using Dapper;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Persistence;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.GetInventarioEstadoById;

public sealed class GetInventarioEstadoByIdQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetInventarioEstadoByIdQuery, InventarioEstadoDto>
{
    public async Task<InventarioEstadoDto> Handle(GetInventarioEstadoByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Nombre
            FROM InventarioEstados
            WHERE Id = @Id
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var estado = await connection.QuerySingleOrDefaultAsync<InventarioEstadoRow>(new CommandDefinition(sql, new { request.Id }, cancellationToken: cancellationToken))
            ?? throw new KeyNotFoundException("El estado de inventario no existe.");

        return new InventarioEstadoDto(estado.Id, estado.Nombre);
    }

    private sealed class InventarioEstadoRow
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
