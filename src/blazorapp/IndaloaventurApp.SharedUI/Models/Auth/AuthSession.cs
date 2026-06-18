namespace IndaloaventurApp.SharedUI.Models.Auth;

public sealed record AuthSession
{
    public AuthSession(
        string accessToken,
        string tokenType,
        int expiresInSeconds,
        bool isMember,
        IReadOnlyList<string>? roles = null,
        Guid? userId = null)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresInSeconds = expiresInSeconds;
        IsMember = isMember;
        Roles = roles?
            .Where(static role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
            ?? Array.Empty<string>();
        UserId = userId;
    }

    public string AccessToken { get; }

    public string TokenType { get; }

    public int ExpiresInSeconds { get; }

    public bool IsMember { get; }

    public IReadOnlyList<string> Roles { get; }

    public Guid? UserId { get; }

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        return Roles.Any(currentRole => string.Equals(currentRole, role, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanAccessOwnFederativeLicenses()
    {
        return IsMember && IsInRole("Member");
    }

    public bool CanAdministerFederativeLicenses()
    {
        return IsInRole("Admin");
    }

    public bool CanAccessFederativeLicenses()
    {
        return CanAccessOwnFederativeLicenses() || CanAdministerFederativeLicenses();
    }
}
