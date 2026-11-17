using Dapper;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.Persistence;
using MediatR;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetTiposLicencia;

public sealed class GetTiposLicenciaQueryHandler(IQueryConnectionFactory connectionFactory)
    : IRequestHandler<GetTiposLicenciaQuery, IReadOnlyCollection<TipoLicenciaDto>>
{
    public async Task<IReadOnlyCollection<TipoLicenciaDto>> Handle(GetTiposLicenciaQuery request, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<TipoRow>(new CommandDefinition(
            "SELECT Id, Nombre FROM TiposLicencias ORDER BY Id", cancellationToken: cancellationToken));
        return rows.Select(x => new TipoLicenciaDto(x.Id, x.Nombre)).ToArray();
    }

    private sealed class TipoRow
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
