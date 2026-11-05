using Dapper;
using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.GetInventarioMaterialById;

public sealed class GetInventarioMaterialByIdQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetInventarioMaterialByIdQuery, InventarioMaterialDto>
{
    public async Task<InventarioMaterialDto> Handle(
        GetInventarioMaterialByIdQuery request,
        CancellationToken cancellationToken)
    {
        const string sql = """
            WITH CantidadesComprometidas AS (
                SELECT InventarioMaterialId, SUM(CantidadSolicitada) AS CantidadComprometida
                FROM ReservasMaterial
                WHERE Estado IN (@EstadoSolicitada, @EstadoRetirada)
                GROUP BY InventarioMaterialId
            )
            SELECT m.Id, m.Descripcion, m.MarcaModelo, m.Cantidad, m.CategoriaId,
                   c.Nombre AS CategoriaNombre, c.CategoriaPadreId,
                   p.Nombre AS CategoriaPadreNombre, m.EstadoId,
                   e.Nombre AS EstadoNombre, m.FechaAdquisicion, m.AdquiridoDonado, m.Activo,
                   m.Observaciones, m.NumeroSerie,
                   m.Cantidad - COALESCE(r.CantidadComprometida, 0) AS CantidadDisponible
            FROM InventarioMateriales AS m
            INNER JOIN InventarioCategorias AS c ON c.Id = m.CategoriaId
            LEFT JOIN InventarioCategorias AS p ON p.Id = c.CategoriaPadreId
            INNER JOIN InventarioEstados AS e ON e.Id = m.EstadoId
            LEFT JOIN CantidadesComprometidas AS r ON r.InventarioMaterialId = m.Id
            WHERE m.Id = @Id
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<InventarioMaterialRow>(
            new CommandDefinition(
                sql,
                new
                {
                    request.Id,
                    EstadoSolicitada = (int)EstadoReservaMaterial.Solicitada,
                    EstadoRetirada = (int)EstadoReservaMaterial.Retirada
                },
                cancellationToken: cancellationToken));

        return row is null
            ? throw new KeyNotFoundException("El material de inventario no existe.")
            : ToDto(row);
    }

    private static InventarioMaterialDto ToDto(InventarioMaterialRow row) => new(
        row.Id,
        row.Descripcion,
        row.MarcaModelo,
        row.Cantidad,
        row.CategoriaId,
        row.CategoriaNombre,
        row.CategoriaPadreId,
        row.CategoriaPadreNombre,
        row.EstadoId,
        row.EstadoNombre,
        ToDateOnly(row.FechaAdquisicion),
        row.AdquiridoDonado,
        row.Observaciones,
        row.NumeroSerie,
        row.Activo,
        Math.Max(0, row.CantidadDisponible));

    private static DateOnly? ToDateOnly(object? value) => value switch
    {
        null or DBNull => null,
        DateOnly date => date,
        DateTime dateTime => DateOnly.FromDateTime(dateTime),
        DateTimeOffset dateTimeOffset => DateOnly.FromDateTime(dateTimeOffset.DateTime),
        string text => DateOnly.Parse(text, System.Globalization.CultureInfo.InvariantCulture),
        _ => throw new InvalidOperationException($"No se puede convertir el valor de tipo '{value.GetType().Name}' a DateOnly.")
    };

    private sealed class InventarioMaterialRow
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string? MarcaModelo { get; set; }
        public int Cantidad { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public int? CategoriaPadreId { get; set; }
        public string? CategoriaPadreNombre { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
        public object? FechaAdquisicion { get; set; }
        public string AdquiridoDonado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string? NumeroSerie { get; set; }
        public bool Activo { get; set; }
        public int CantidadDisponible { get; set; }
    }
}
