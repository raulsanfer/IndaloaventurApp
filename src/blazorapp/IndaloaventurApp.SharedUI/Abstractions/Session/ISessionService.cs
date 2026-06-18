namespace IndaloaventurApp.SharedUI.Abstractions.Session;

using IndaloaventurApp.SharedUI.Models.Auth;

public interface ISessionService
{
    bool IsInitialized { get; }

    bool IsAuthenticated { get; }

    AuthSession? CurrentSession { get; }

    Task EnsureInitializedAsync();

    Task SetSessionAsync(AuthSession session, bool rememberMe);

    Task SignOutAsync();
}
