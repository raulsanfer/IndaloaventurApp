namespace IndaloaventurApp.SharedUI.Abstractions.Auth;

using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;

/// <summary>
/// Provides authentication operations consumed by login and password recovery components.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with email or username and password.
    /// </summary>
    Task<ServiceResult<AuthSession>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user through an external identity provider token.
    /// </summary>
    Task<ServiceResult<AuthSession>> LoginSocialAsync(SocialLoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a password recovery flow for the supplied email address.
    /// </summary>
    Task<ServiceResult<string>> RequestPasswordRecoveryAsync(PasswordRecoveryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a password using a recovery token issued by the backend.
    /// </summary>
    Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}
