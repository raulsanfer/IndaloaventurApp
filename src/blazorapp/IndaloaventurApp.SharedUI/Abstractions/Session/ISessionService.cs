namespace IndaloaventurApp.SharedUI.Abstractions.Session;

using IndaloaventurApp.SharedUI.Models.Auth;

/// <summary>
/// Stores, restores and clears the authenticated frontend session.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Gets whether persisted session restoration has completed.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Gets whether the current session can be used as authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current authenticated session, when one is available.
    /// </summary>
    AuthSession? CurrentSession { get; }

    /// <summary>
    /// Restores any persisted browser session before protected views decide navigation.
    /// </summary>
    Task EnsureInitializedAsync();

    /// <summary>
    /// Stores the supplied session in memory and in the selected browser storage.
    /// </summary>
    Task SetSessionAsync(AuthSession session, bool rememberMe);

    /// <summary>
    /// Clears the active and persisted session.
    /// </summary>
    Task SignOutAsync();
}
