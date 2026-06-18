namespace IndaloAventurApi.Infrastructure.Security;

public sealed class SocialAuthOptions
{
    public const string SectionName = "SocialAuth";

    public string GoogleAudience { get; init; } = string.Empty;
}
