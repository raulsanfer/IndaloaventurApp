using Dapper;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Persistence;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.GetAllInventarioCategorias;

public sealed class GetAllInventarioCategoriasQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetAllInventarioCategoriasQuery, IReadOnlyCollection<InventarioCategoriaDto>>
{
    public async Task<IReadOnlyCollection<InventarioCategoriaDto>> Handle(GetAllInventarioCategoriasQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Nombre, CategoriaPadreId
            FROM InventarioCategorias
            ORDER BY Nombre ASC, Id ASC
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = (await connection.QueryAsync<InventarioCategoriaRow>(new CommandDefinition(sql, cancellationToken: cancellationToken))).ToArray();

        return rows
            .Where(x => !x.CategoriaPadreId.HasValue)
            .Select(x => new InventarioCategoriaDto(
                x.Id,
                x.Nombre,
                null,
                rows.Where(child => child.CategoriaPadreId == x.Id)
                    .Select(child => new InventarioSubcategoriaDto(child.Id, child.Nombre))
                    .ToArray()))
            .ToArray();
    }

    private sealed class InventarioCategoriaRow
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? CategoriaPadreId { get; set; }
    }
}
