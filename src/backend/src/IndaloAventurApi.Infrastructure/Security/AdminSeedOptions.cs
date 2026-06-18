namespace IndaloAventurApi.Infrastructure.Security;

public sealed class AdminSeedOptions
{
    public const string SectionName = "AdminSeed";

    public string Email { get; init; } = "admin@indaloaventura.local";
    public string Password { get; init; } = "Admin1234A";
}
