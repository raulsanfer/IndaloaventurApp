using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.FichasContacto.ValueObjects;

public sealed class ObservacionesContacto
{
    public const int MaxLength = 500;

    private ObservacionesContacto(string value)
    {
        Value = value;
    }

    private ObservacionesContacto()
    {
        Value = string.Empty;
    }

    public string Value { get; private set; }

    public static ObservacionesContacto? CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > MaxLength)
        {
            throw new DomainException($"Las observaciones no pueden superar {MaxLength} caracteres.");
        }

        return new ObservacionesContacto(normalized);
    }
}