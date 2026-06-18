using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.FichasContacto.ValueObjects;

public sealed class DireccionContacto
{
    public const int MaxLength = 250;

    private DireccionContacto(string value)
    {
        Value = value;
    }

    private DireccionContacto()
    {
        Value = string.Empty;
    }

    public string Value { get; private set; }

    public static DireccionContacto? CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > MaxLength)
        {
            throw new DomainException($"La direccion no puede superar {MaxLength} caracteres.");
        }

        return new DireccionContacto(normalized);
    }
}
