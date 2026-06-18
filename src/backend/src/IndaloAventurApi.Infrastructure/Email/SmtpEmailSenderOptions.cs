namespace IndaloAventurApi.Infrastructure.Email;

public sealed class SmtpEmailSenderOptions
{
    public const string SectionName = "Email";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 25;
    public string FromAddress { get; init; } = string.Empty;
    public string FromName { get; init; } = "Club IndaloAventura";
    public string? Username { get; init; }
    public string? Password { get; init; }
    public bool EnableSsl { get; init; } = true;
}
