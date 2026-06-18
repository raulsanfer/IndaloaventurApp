using System.Net.Mail;
using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.FichasContacto.ValueObjects;

public sealed class EmailContacto
{
    public const int MaxLength = 254;

    private EmailContacto(string value)
    {
        Value = value;
    }

    private EmailContacto()
    {
        Value = string.Empty;
    }

    public string Value { get; private set; }

    public static EmailContacto? CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > MaxLength)
        {
            throw new DomainException($"El email no puede superar {MaxLength} caracteres.");
        }

        try
        {
            var address = new MailAddress(normalized);
            if (!string.Equals(address.Address, normalized, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException("El email tiene un formato invalido.");
            }
        }
        catch (FormatException)
        {
            throw new DomainException("El email tiene un formato invalido.");
        }

        return new EmailContacto(normalized);
    }
}
