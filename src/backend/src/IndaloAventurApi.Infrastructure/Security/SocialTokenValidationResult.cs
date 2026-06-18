namespace IndaloAventurApi.Infrastructure.Security;

public sealed record SocialTokenValidationResult(
    bool Succeeded,
    string Provider,
    string ProviderUserId,
    string Email,
    IEnumerable<string> Errors)
{
    public static SocialTokenValidationResult Failed(params string[] errors)
        => new(false, string.Empty, string.Empty, string.Empty, errors);
}
