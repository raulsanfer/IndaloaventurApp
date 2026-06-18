using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.FichasContacto.ValueObjects;

public sealed class NombreContacto
{
    public const int MaxLength = 120;

    private NombreContacto(string value)
    {
        Value = value;
    }

    private NombreContacto()
    {
        Value = string.Empty;
    }

    public string Value { get; private set; }

    public static NombreContacto Create(string value)
    {
        var normalized = (value ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainException("El nombre del contacto es obligatorio.");
        }

        if (normalized.Length > MaxLength)
        {
            throw new DomainException($"El nombre del contacto no puede superar {MaxLength} caracteres.");
        }

        return new NombreContacto(normalized);
    }
}