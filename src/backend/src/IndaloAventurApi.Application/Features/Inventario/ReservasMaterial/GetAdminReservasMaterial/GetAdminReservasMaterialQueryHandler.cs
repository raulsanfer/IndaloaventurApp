using Dapper;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Common;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservasMaterial;

public sealed class GetAdminReservasMaterialQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetAdminReservasMaterialQuery, IReadOnlyCollection<ReservaMaterialDto>>
{
    public async Task<IReadOnlyCollection<ReservaMaterialDto>> Handle(GetAdminReservasMaterialQuery request, CancellationToken cancellationToken)
    {
        ReservaMaterialInventoryManagementAccess.RequireInventoryManagement(request.CanManageInventory);

        var sql = $"""
            {ReservaMaterialDapperProjection.SelectSql}
            ORDER BY r.FechaSolicitudUtc DESC, r.Id DESC
            """;
        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<ReservaMaterialProjectionRow>(new CommandDefinition(
            sql,
            ReservaMaterialDapperProjection.EstadoParameters,
            cancellationToken: cancellationToken));

        return rows.Select(ReservaMaterialDapperProjection.ToDto).ToArray();
    }
}
