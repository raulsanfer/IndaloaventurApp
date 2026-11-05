using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.Inventario;

public static class InventarioMaterialTexto
{
    public const int DescripcionMaxLength = 250;
    public const int MarcaModeloMaxLength = 200;
    public const int AdquiridoDonadoMaxLength = 250;
    public const int ObservacionesMaxLength = 2000;
    public const int NumeroSerieMaxLength = 120;

    public static string Required(string? value, string fieldName, int maxLength, bool allowNewLines = false)
    {
        var normalized = Normalize(value, fieldName, maxLength, allowNewLines);
        if (string.IsNullOrWhiteSpace(normalized)) throw new DomainException($"El campo {fieldName} es obligatorio.");
        return normalized;
    }

    public static string? Optional(string? value, string fieldName, int maxLength, bool allowNewLines = false)
        => string.IsNullOrWhiteSpace(value) ? null : Normalize(value, fieldName, maxLength, allowNewLines);

    public static string NormalizeSerial(string value) => value.ToUpperInvariant();

    private static string Normalize(string? value, string fieldName, int maxLength, bool allowNewLines)
    {
        var normalized = (value ?? string.Empty).Replace("\r\n", "\n").Trim();
        if (normalized.Length > maxLength) throw new DomainException($"El campo {fieldName} no puede superar {maxLength} caracteres.");
        if (normalized.Any(character => char.IsControl(character) && (!allowNewLines || character != '\n')))
            throw new DomainException($"El campo {fieldName} contiene caracteres no permitidos.");
        return normalized;
    }
}
