namespace IndaloaventurApp.SharedUI.Abstractions.Auth;

using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;

public interface IAuthService
{
    Task<ServiceResult<AuthSession>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<AuthSession>> LoginSocialAsync(SocialLoginRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> RequestPasswordRecoveryAsync(PasswordRecoveryRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}
