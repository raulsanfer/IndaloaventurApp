using Dapper;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Common;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMisReservasMaterial;

public sealed class GetMisReservasMaterialQueryHandler(
    IQueryConnectionFactory queryConnectionFactory,
    ISocioReservaReader socioReader)
    : IRequestHandler<GetMisReservasMaterialQuery, IReadOnlyCollection<ReservaMaterialDto>>
{
    public async Task<IReadOnlyCollection<ReservaMaterialDto>> Handle(GetMisReservasMaterialQuery request, CancellationToken cancellationToken)
    {
        await ReservaMaterialMemberAccess.RequireSocioAsync(
            request.AuthenticatedUserId,
            request.IsMember,
            socioReader,
            "consultar",
            cancellationToken);

        var sql = $"""
            {ReservaMaterialDapperProjection.SelectSql}
            WHERE r.UserId = @UserId
            ORDER BY r.FechaSolicitudUtc DESC, r.Id DESC
            """;
        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<ReservaMaterialProjectionRow>(new CommandDefinition(
            sql,
            new
            {
                UserId = request.AuthenticatedUserId,
                EstadoSolicitada = (int)IndaloAventurApi.Domain.Inventario.ReservasMaterial.EstadoReservaMaterial.Solicitada,
                EstadoRetirada = (int)IndaloAventurApi.Domain.Inventario.ReservasMaterial.EstadoReservaMaterial.Retirada
            },
            cancellationToken: cancellationToken));

        return rows.Select(ReservaMaterialDapperProjection.ToDto).ToArray();
    }
}
