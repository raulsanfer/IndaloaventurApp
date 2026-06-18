using Dapper;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Domain.LicenciasFederativas;
using MediatR;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetMisSolicitudesLicenciaFederativa;

public sealed class GetMisSolicitudesLicenciaFederativaQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetMisSolicitudesLicenciaFederativaQuery, IReadOnlyCollection<SolicitudLicenciaFederativaDto>>
{
    public async Task<IReadOnlyCollection<SolicitudLicenciaFederativaDto>> Handle(GetMisSolicitudesLicenciaFederativaQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                s.Id,
                s.Temporada,
                s.Estado,
                s.FechaCreacionUtc,
                t.Id AS TarifaLicenciaFederativaId,
                t.Licencia,
                t.Categoria,
                t.Territorio,
                t.MediaTemporada,
                t.PrecioClub,
                t.PrecioIndependiente
            FROM SolicitudesLicenciasFederativas s
            INNER JOIN TarifasLicenciasFederativas t ON t.Id = s.TarifaLicenciaFederativaId
            WHERE s.UserId = @UserId
            ORDER BY s.FechaCreacionUtc DESC, s.Temporada DESC
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<SolicitudLicenciaFederativaRow>(new CommandDefinition(
            sql,
            new { UserId = request.AuthenticatedUserId },
            cancellationToken: cancellationToken));

        return rows
            .Select(Map)
            .ToArray();
    }

    private static SolicitudLicenciaFederativaDto Map(SolicitudLicenciaFederativaRow row)
    {
        var estado = Enum.IsDefined(typeof(EstadoSolicitudLicenciaFederativa), row.Estado)
            ? ((EstadoSolicitudLicenciaFederativa)row.Estado).ToString()
            : row.Estado.ToString();

        return new SolicitudLicenciaFederativaDto(
            row.Id,
            row.Temporada,
            estado,
            row.FechaCreacionUtc,
            row.TarifaLicenciaFederativaId,
            row.Licencia,
            row.Categoria,
            row.Territorio,
            row.MediaTemporada,
            row.PrecioClub,
            row.PrecioIndependiente);
    }

    private sealed record SolicitudLicenciaFederativaRow(
        Guid Id,
        int Temporada,
        int Estado,
        DateTime FechaCreacionUtc,
        int TarifaLicenciaFederativaId,
        string Licencia,
        string Categoria,
        string Territorio,
        bool MediaTemporada,
        decimal PrecioClub,
        decimal? PrecioIndependiente);
}
