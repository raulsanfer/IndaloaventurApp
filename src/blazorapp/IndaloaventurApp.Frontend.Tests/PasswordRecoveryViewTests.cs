namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Components.Login;
using IndaloaventurApp.SharedUI.Models.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public sealed class PasswordRecoveryViewTests : BunitContext
{
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
