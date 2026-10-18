using Dapper;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Persistence;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.GetInventarioCategoriaById;

public sealed class GetInventarioCategoriaByIdQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetInventarioCategoriaByIdQuery, InventarioCategoriaDto>
{
    public async Task<InventarioCategoriaDto> Handle(GetInventarioCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Nombre, CategoriaPadreId
            FROM InventarioCategorias
            WHERE Id = @Id;

            SELECT Id, Nombre
            FROM InventarioCategorias
            WHERE CategoriaPadreId = @Id
            ORDER BY Nombre ASC, Id ASC;
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(sql, new { request.Id }, cancellationToken: cancellationToken));
        var categoria = await grid.ReadSingleOrDefaultAsync<InventarioCategoriaRow>();
        if (categoria is null)
        {
            throw new KeyNotFoundException("La categoria de inventario no existe.");
        }

        var subcategorias = (await grid.ReadAsync<InventarioSubcategoriaRow>())
            .Select(x => new InventarioSubcategoriaDto(x.Id, x.Nombre))
            .ToArray();

        return new InventarioCategoriaDto(categoria.Id, categoria.Nombre, categoria.CategoriaPadreId, subcategorias);
    }

    private sealed class InventarioCategoriaRow
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? CategoriaPadreId { get; set; }
    }

    private sealed class InventarioSubcategoriaRow
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
