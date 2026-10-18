using Dapper;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Domain.LicenciasFederativas;
using MediatR;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetMiSolicitudLicenciaFederativa;

public sealed class GetMiSolicitudLicenciaFederativaQueryHandler(IQueryConnectionFactory queryConnectionFactory)
    : IRequestHandler<GetMiSolicitudLicenciaFederativaQuery, SolicitudLicenciaFederativaDto>
{
    public async Task<SolicitudLicenciaFederativaDto> Handle(GetMiSolicitudLicenciaFederativaQuery request, CancellationToken cancellationToken)
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
            WHERE s.Id = @SolicitudId AND s.UserId = @UserId
            """;

        using var connection = await queryConnectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<SolicitudLicenciaFederativaRow>(new CommandDefinition(
            sql,
            new { request.SolicitudId, UserId = request.AuthenticatedUserId },
            cancellationToken: cancellationToken));

        if (row is null)
        {
            throw new KeyNotFoundException("La solicitud de licencia federativa no existe.");
        }

        var estado = Enum.IsDefined(typeof(EstadoSolicitudLicenciaFederativa), row.Estado)
            ? ((EstadoSolicitudLicenciaFederativa)row.Estado).ToString()
            : row.Estado.ToString();

        return new SolicitudLicenciaFederativaDto(
            ParseGuid(row.Id),
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

    private static Guid ParseGuid(object value) => value switch
    {
        Guid guid => guid,
        string text => Guid.Parse(text),
        byte[] bytes => new Guid(bytes),
        _ => throw new InvalidOperationException("El identificador de la solicitud no tiene un formato valido.")
    };

    private sealed class SolicitudLicenciaFederativaRow
    {
        public object Id { get; set; } = default!;
        public int Temporada { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacionUtc { get; set; }
        public int TarifaLicenciaFederativaId { get; set; }
        public string Licencia { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Territorio { get; set; } = string.Empty;
        public bool MediaTemporada { get; set; }
        public decimal PrecioClub { get; set; }
        public decimal? PrecioIndependiente { get; set; }
    }
}
