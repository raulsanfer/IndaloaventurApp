namespace IndaloaventurApp.Web.Client.Infrastructure.Session;

using IndaloaventurApp.SharedUI.Models.Auth;

internal sealed record PersistedSessionSnapshot(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds,
    bool IsMember,
    Guid? UserId,
    IReadOnlyList<string> Roles,
    DateTimeOffset PersistedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    bool RememberMe)
{
    public static PersistedSessionSnapshot Create(AuthSession session, bool rememberMe, DateTimeOffset nowUtc)
    {
        var expiresInSeconds = Math.Max(session.ExpiresInSeconds, 0);

        return new PersistedSessionSnapshot(
            session.AccessToken,
            session.TokenType,
            expiresInSeconds,
            session.IsMember,
            session.UserId,
            session.Roles.ToArray(),
            nowUtc,
            nowUtc.AddSeconds(expiresInSeconds),
            rememberMe);
    }

    public bool IsExpired(DateTimeOffset nowUtc)
    {
        return ExpiresAtUtc <= nowUtc;
    }

    public AuthSession ToAuthSession(DateTimeOffset nowUtc)
    {
        var remainingSeconds = Math.Max(0, (int)Math.Ceiling((ExpiresAtUtc - nowUtc).TotalSeconds));

        return new AuthSession(
            AccessToken,
            TokenType,
            remainingSeconds,
            IsMember,
            Roles,
            UserId);
    }
}
