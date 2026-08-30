namespace IndaloaventurApp.Frontend.Tests.Features.Auth;

using Bunit;
using IndaloaventurApp.SharedUI.Components.Login;
using IndaloaventurApp.SharedUI.Models.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public sealed class PasswordRecoveryViewTests : BunitContext
{
    /// <summary>
    /// Covers scenario: password recovery request view submits email and shows neutral message.
    /// </summary>
    [Fact]
    public void PasswordRecoveryRequestView_SubmitsEmail_AndShowsNeutralMessage()
    {
        var authService = new RecordingAuthService
        {
            PasswordRecoveryHandler = request => Task.FromResult(ServiceResult<string>.Success(
                $"Si existe una cuenta asociada a {request.Email}, te hemos enviado instrucciones."))
        };

        Services.AddSingletonAuthDependencies(authService, new RecordingSessionService(), string.Empty);

        var cut = Render<PasswordRecoveryRequestView>();

        cut.Find("#recoveryEmail").Change("member@club.test");
        cut.Find("form.login-form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(authService.LastPasswordRecoveryRequest);
            Assert.Equal("member@club.test", authService.LastPasswordRecoveryRequest.Email);
            Assert.Contains("Si existe una cuenta asociada a member@club.test", cut.Markup);
        });
    }

    /// <summary>
    /// Covers scenario: reset password view shows restart action when link is incomplete.
    /// </summary>
    [Fact]
    public void ResetPasswordView_ShowsRestartAction_WhenLinkIsIncomplete()
    {
        var authService = new RecordingAuthService();
        Services.AddSingletonAuthDependencies(authService, new RecordingSessionService(), string.Empty);

        var cut = Render<ResetPasswordView>(parameters => parameters
            .Add(x => x.Email, "member@club.test"));

        Assert.Contains("auth_reset_missing_link", cut.Markup);
        Assert.Equal("/forgot-password", cut.Find(".login-panel__secondary-actions a").GetAttribute("href"));
        Assert.Null(authService.LastResetPasswordRequest);
    }

    /// <summary>
    /// Covers scenario: reset password view shows backend error and restart link.
    /// </summary>
    [Fact]
    public void ResetPasswordView_ShowsBackendError_AndRestartLink()
    {
        var authService = new RecordingAuthService
        {
            ResetPasswordHandler = _ => Task.FromResult(ServiceResult<string>.Failure(
                new ServiceError("auth.reset_password_failed", "El token ha expirado o no es válido.")))
        };

        Services.AddSingletonAuthDependencies(authService, new RecordingSessionService(), string.Empty);

        var cut = Render<ResetPasswordView>(parameters => parameters
            .Add(x => x.Email, "member@club.test")
            .Add(x => x.Token, "valid-token"));

        cut.Find("#newPassword").Change("NuevaClave123A");
        cut.Find("#confirmPassword").Change("NuevaClave123A");
        cut.Find("form.login-form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(authService.LastResetPasswordRequest);
            Assert.Contains("El token ha expirado o no es válido.", cut.Markup);
            Assert.Equal("/forgot-password", cut.Find(".login-panel__secondary-actions a").GetAttribute("href"));
        });
    }

    /// <summary>
    /// Covers scenario: reset password view redirects to login with success flag on success.
    /// </summary>
    [Fact]
    public void ResetPasswordView_RedirectsToLoginWithSuccessFlag_OnSuccess()
    {
        var authService = new RecordingAuthService
        {
            ResetPasswordHandler = _ => Task.FromResult(ServiceResult<string>.Success("La contraseña se ha actualizado correctamente."))
        };

        Services.AddSingletonAuthDependencies(authService, new RecordingSessionService(), string.Empty);
        var navigationManager = Services.GetRequiredService<NavigationManager>();

        var cut = Render<ResetPasswordView>(parameters => parameters
            .Add(x => x.Email, "member@club.test")
            .Add(x => x.Token, "valid-token"));

        cut.Find("#newPassword").Change("NuevaClave123A");
        cut.Find("#confirmPassword").Change("NuevaClave123A");
        cut.Find("form.login-form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("http://localhost/?passwordReset=success", navigationManager.Uri);
        });
    }
}
