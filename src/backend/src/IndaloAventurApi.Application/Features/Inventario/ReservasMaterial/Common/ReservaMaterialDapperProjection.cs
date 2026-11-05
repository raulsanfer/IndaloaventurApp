using System.Globalization;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Common;

internal static class ReservaMaterialDapperProjection
{
    public const string SelectSql = """
        WITH CantidadesComprometidas AS (
            SELECT InventarioMaterialId, SUM(CantidadSolicitada) AS CantidadComprometida
            FROM ReservasMaterial
            WHERE Estado IN (@EstadoSolicitada, @EstadoRetirada)
            GROUP BY InventarioMaterialId
        )
        SELECT
            r.Id,
            r.UserId,
            r.InventarioMaterialId,
            m.Descripcion AS MaterialDescripcion,
            r.CantidadSolicitada,
            r.FechaSolicitudUtc,
            r.FechaRetiradaPrevista,
            r.FechaRetirada,
            r.FechaDevolucionEstimada,
            r.FechaDevolucionEfectiva,
            r.Observaciones,
            r.ObservacionAdministracion,
            r.Estado,
            r.ConfirmacionDevolucionSocio,
            r.ConfirmacionDevolucionAdmin,
            f.Nombre AS SocioNombre,
            f.Apellidos AS SocioApellidos,
            f.Email AS SocioEmail,
            m.Cantidad AS CantidadTotal,
            COALESCE(c.CantidadComprometida, 0) AS CantidadComprometida,
            m.Activo
        FROM ReservasMaterial r
        INNER JOIN InventarioMateriales m ON m.Id = r.InventarioMaterialId
        INNER JOIN FichasSocio f ON f.UserId = r.UserId
        LEFT JOIN CantidadesComprometidas c ON c.InventarioMaterialId = r.InventarioMaterialId
        """;

    public static readonly object EstadoParameters = new
    {
        EstadoSolicitada = (int)EstadoReservaMaterial.Solicitada,
        EstadoRetirada = (int)EstadoReservaMaterial.Retirada
    };

    public static ReservaMaterialDto ToDto(ReservaMaterialProjectionRow row)
    {
        var cantidadTotal = ToInt(row.CantidadTotal, "La cantidad total del material no tiene un formato valido.");
        var cantidadComprometida = ToInt(row.CantidadComprometida, "La cantidad comprometida no tiene un formato valido.");
        var estado = ToEnum<EstadoReservaMaterial>(row.Estado, "El estado de la reserva no es valido.");

        return new ReservaMaterialDto(
            ToGuid(row.Id, "El identificador de la reserva no tiene un formato valido."),
            ToGuid(row.UserId, "El identificador del socio no tiene un formato valido."),
            ToInt(row.InventarioMaterialId, "El identificador del material no tiene un formato valido."),
            row.MaterialDescripcion ?? string.Empty,
            ToInt(row.CantidadSolicitada, "La cantidad solicitada no tiene un formato valido."),
            ToDateTimeUtc(row.FechaSolicitudUtc, "La fecha de solicitud no tiene un formato valido."),
            ToDateOnly(row.FechaRetiradaPrevista, "La fecha de retirada prevista no tiene un formato valido."),
            ToNullableDateOnly(row.FechaRetirada, "La fecha de retirada no tiene un formato valido."),
            ToDateOnly(row.FechaDevolucionEstimada, "La fecha de devolucion estimada no tiene un formato valido."),
            ToNullableDateOnly(row.FechaDevolucionEfectiva, "La fecha de devolucion efectiva no tiene un formato valido."),
            row.Observaciones,
            row.ObservacionAdministracion,
            estado,
            ToBool(row.ConfirmacionDevolucionSocio, "La confirmacion de devolucion del socio no tiene un formato valido."),
            ToBool(row.ConfirmacionDevolucionAdmin, "La confirmacion de devolucion de administracion no tiene un formato valido."),
            new SocioReservaDto(
                ToGuid(row.UserId, "El identificador del socio no tiene un formato valido."),
                string.Join(' ', new[] { row.SocioNombre, row.SocioApellidos }.Where(value => !string.IsNullOrWhiteSpace(value))),
                row.SocioEmail ?? string.Empty),
            new DisponibilidadMaterialReservaDto(
                ToInt(row.InventarioMaterialId, "El identificador del material no tiene un formato valido."),
                cantidadTotal,
                cantidadComprometida,
                cantidadTotal - cantidadComprometida,
                ToBool(row.Activo, "El indicador de material activo no tiene un formato valido.")));
    }

    private static Guid ToGuid(object? value, string error) => value switch
    {
        Guid guid => guid,
        string text when Guid.TryParse(text, out var guid) => guid,
        byte[] bytes when bytes.Length == 16 => new Guid(bytes),
        _ => throw new InvalidOperationException(error)
    };

    private static int ToInt(object? value, string error)
    {
        try
        {
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }
        catch (Exception) when (value is not null)
        {
            throw new InvalidOperationException(error);
        }
    }

    private static bool ToBool(object? value, string error) => value switch
    {
        bool result => result,
        byte result => result != 0,
        short result => result != 0,
        int result => result != 0,
        long result => result != 0,
        string text when bool.TryParse(text, out var result) => result,
        "0" => false,
        "1" => true,
        _ => throw new InvalidOperationException(error)
    };

    private static TEnum ToEnum<TEnum>(object? value, string error) where TEnum : struct, Enum
    {
        var numericValue = ToInt(value, error);
        return Enum.IsDefined(typeof(TEnum), numericValue)
            ? (TEnum)Enum.ToObject(typeof(TEnum), numericValue)
            : throw new InvalidOperationException(error);
    }

    private static DateTime ToDateTimeUtc(object? value, string error) => value switch
    {
        DateTime dateTime => dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
        DateTimeOffset offset => offset.UtcDateTime,
        string text when DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime) => dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
        _ => throw new InvalidOperationException(error)
    };

    private static DateOnly ToDateOnly(object? value, string error) => ToNullableDateOnly(value, error)
        ?? throw new InvalidOperationException(error);

    private static DateOnly? ToNullableDateOnly(object? value, string error) => value switch
    {
        null or DBNull => null,
        DateOnly dateOnly => dateOnly,
        DateTime dateTime => DateOnly.FromDateTime(dateTime),
        DateTimeOffset offset => DateOnly.FromDateTime(offset.DateTime),
        string text when DateOnly.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly) => dateOnly,
        string text when DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime) => DateOnly.FromDateTime(dateTime),
        _ => throw new InvalidOperationException(error)
    };
}

internal sealed class ReservaMaterialProjectionRow
{
    public object? Id { get; set; }
    public object? UserId { get; set; }
    public object? InventarioMaterialId { get; set; }
    public string? MaterialDescripcion { get; set; }
    public object? CantidadSolicitada { get; set; }
    public object? FechaSolicitudUtc { get; set; }
    public object? FechaRetiradaPrevista { get; set; }
    public object? FechaRetirada { get; set; }
    public object? FechaDevolucionEstimada { get; set; }
    public object? FechaDevolucionEfectiva { get; set; }
    public string? Observaciones { get; set; }
    public string? ObservacionAdministracion { get; set; }
    public object? Estado { get; set; }
    public object? ConfirmacionDevolucionSocio { get; set; }
    public object? ConfirmacionDevolucionAdmin { get; set; }
    public string? SocioNombre { get; set; }
    public string? SocioApellidos { get; set; }
    public string? SocioEmail { get; set; }
    public object? CantidadTotal { get; set; }
    public object? CantidadComprometida { get; set; }
    public object? Activo { get; set; }
}
