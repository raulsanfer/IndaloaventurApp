using System.Globalization;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.Common;

internal static class LicenseDateMapping
{
    // SQL Server returns date as DateTime; SQLite test connections return text.
    public static DateOnly? ToDateOnly(object? value) => value switch
    {
        null or DBNull => null,
        DateOnly date => date,
        DateTime date => DateOnly.FromDateTime(date),
        string text => DateOnly.Parse(text, CultureInfo.InvariantCulture),
        _ => throw new InvalidOperationException("La fecha de cobertura no tiene un formato válido.")
    };
}
