using Dapper;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Domain.LicenciasFederativas;
using MediatR;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetAdminSolicitudesLicenciaFederativa;

public sealed class GetAdminSolicitudesLicenciaFederativaQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetAdminSolicitudesLicenciaFederativaQuery, IReadOnlyCollection<AdminSolicitudLicenciaFederativaDto>>
{
    public async Task<IReadOnlyCollection<AdminSolicitudLicenciaFederativaDto>> Handle(GetAdminSolicitudesLicenciaFederativaQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                s.Id,
                s.UserId,
                COALESCE(u.Email, '') AS UserEmail,
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
            LEFT JOIN AspNetUsers u ON u.Id = s.UserId
            WHERE (@UserId IS NULL OR s.UserId = @UserId)
              AND (@Temporada IS NULL OR s.Temporada = @Temporada)
              AND (@Estado IS NULL OR s.Estado = @Estado)
            ORDER BY s.FechaCreacionUtc DESC, s.Temporada DESC, u.Email ASC
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<AdminSolicitudLicenciaFederativaRow>(new CommandDefinition(
            sql,
            new
            {
                request.UserId,
                request.Temporada,
                Estado = request.Estado.HasValue ? (int?)request.Estado.Value : null
            },
            cancellationToken: cancellationToken));

        return rows.Select(Map).ToArray();
    }

    private static AdminSolicitudLicenciaFederativaDto Map(AdminSolicitudLicenciaFederativaRow row)
    {
        var estado = Enum.IsDefined(typeof(EstadoSolicitudLicenciaFederativa), row.Estado)
            ? ((EstadoSolicitudLicenciaFederativa)row.Estado).ToString()
            : row.Estado.ToString();

        return new AdminSolicitudLicenciaFederativaDto(
            row.Id,
            row.UserId,
            row.UserEmail,
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

    private sealed record AdminSolicitudLicenciaFederativaRow(
        Guid Id,
        Guid UserId,
        string UserEmail,
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
