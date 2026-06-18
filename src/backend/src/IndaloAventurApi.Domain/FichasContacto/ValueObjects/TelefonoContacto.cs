using System.Text.RegularExpressions;
using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.FichasContacto.ValueObjects;

public sealed partial class TelefonoContacto
{
    public const int MaxLength = 20;

    private TelefonoContacto(string value)
    {
        Value = value;
    }

    private TelefonoContacto()
    {
        Value = string.Empty;
    }

    public string Value { get; private set; }

    public static TelefonoContacto Create(string value)
    {
        var normalized = Normalize(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainException("El telefono es obligatorio.");
        }

        if (normalized.Length > MaxLength)
        {
            throw new DomainException($"El telefono no puede superar {MaxLength} caracteres.");
        }

        if (!TelefonoRegex().IsMatch(normalized))
        {
            throw new DomainException("El telefono tiene un formato invalido.");
        }

        return new TelefonoContacto(normalized);
    }

    public static TelefonoContacto? CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Create(value);
    }

    private static string Normalize(string? value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        return Regex.Replace(trimmed, "\\s+", " ");
    }

    [GeneratedRegex("^\\+?[0-9 ]{6,20}$")]
    private static partial Regex TelefonoRegex();
}