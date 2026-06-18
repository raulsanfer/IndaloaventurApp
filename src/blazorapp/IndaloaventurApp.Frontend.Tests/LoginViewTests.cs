namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Components.Login;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;

public sealed class LoginViewTests : BunitContext
{
    [Fact]
    public void RendersGoogleButtonAndInitializesInterop_WhenClientIdIsConfigured()
    {
        var authService = new RecordingAuthService();
        var sessionService = new RecordingSessionService();
        Services.AddSingletonAuthDependencies(authService, sessionService, "google-client-id");

        JSInterop.Setup<bool>("indaloGoogleAuth.initializeButton", _ => true).SetResult(true);

        var cut = Render<LoginView>();

        cut.WaitForAssertion(() =>
        {
            var invocation = Assert.Single(JSInterop.Invocations, x => x.Identifier == "indaloGoogleAuth.initializeButton");
            Assert.Equal("google-client-id", invocation.Arguments[1]?.ToString());
        });
    }

    [Fact]
    public async Task HandleGoogleCredentialAsync_CreatesSessionAndInvokesSuccessCallback()
    {
        var authService = new RecordingAuthService
        {
            SocialLoginHandler = _ => Task.FromResult(ServiceResult<AuthSession>.Success(TestSessions.MemberSession))
        };

        var sessionService = new RecordingSessionService();
        Services.AddSingletonAuthDependencies(authService, sessionService, "google-client-id");
        JSInterop.Setup<bool>("indaloGoogleAuth.initializeButton", _ => true).SetResult(true);

        var loginSucceededCalls = 0;
        var cut = Render<LoginView>(parameters => parameters
            .Add(x => x.OnLoginSucceeded, () => loginSucceededCalls++));

        await cut.InvokeAsync(() => cut.Instance.HandleGoogleCredentialAsync("valid-id-token"));

        Assert.NotNull(authService.LastSocialLoginRequest);
        Assert.Equal("google", authService.LastSocialLoginRequest.Provider);
        Assert.Equal("valid-id-token", authService.LastSocialLoginRequest.Token);
        Assert.Equal(TestSessions.MemberSession, sessionService.CurrentSession);
        Assert.True(sessionService.IsAuthenticated);
        Assert.False(sessionService.LastRememberMe);
        Assert.Equal(1, loginSucceededCalls);
    }

    [Fact]
    public async Task HandleGoogleCredentialAsync_UsesRememberMeSelectionForPersistedSession()
    {
        var authService = new RecordingAuthService
        {
            SocialLoginHandler = _ => Task.FromResult(ServiceResult<AuthSession>.Success(TestSessions.MemberSession))
        };

        var sessionService = new RecordingSessionService();
        Services.AddSingletonAuthDependencies(authService, sessionService, "google-client-id");
        JSInterop.Setup<bool>("indaloGoogleAuth.initializeButton", _ => true).SetResult(true);

        var cut = Render<LoginView>();
        cut.Find("input[type=\"checkbox\"]").Change(true);

        await cut.InvokeAsync(() => cut.Instance.HandleGoogleCredentialAsync("valid-id-token"));

        Assert.True(sessionService.LastRememberMe);
    }

    [Fact]
    public async Task SocialLoginFailure_ShowsControlledError_AndClassicLoginStillWorks()
    {
        var authService = new RecordingAuthService
        {
            SocialLoginHandler = _ => Task.FromResult(ServiceResult<AuthSession>.Failure(new ServiceError("auth.social_invalid", "Invalid social token"))),
            LoginHandler = _ => Task.FromResult(ServiceResult<AuthSession>.Success(TestSessions.MemberSession))
        };

        var sessionService = new RecordingSessionService();
        Services.AddSingletonAuthDependencies(authService, sessionService, "google-client-id");
        JSInterop.Setup<bool>("indaloGoogleAuth.initializeButton", _ => true).SetResult(true);

        var loginSucceededCalls = 0;
        var cut = Render<LoginView>(parameters => parameters
            .Add(x => x.OnLoginSucceeded, () => loginSucceededCalls++));

        await cut.InvokeAsync(() => cut.Instance.HandleGoogleCredentialAsync("invalid-id-token"));

        Assert.Contains("login_social_google_failed_error", cut.Markup);

        cut.Find("#emailOrUser").Change("demo@indalo.es");
        cut.Find("#password").Change("indalo123");
        cut.Find("form.login-form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(authService.LastLoginRequest);
            Assert.Equal("demo@indalo.es", authService.LastLoginRequest.EmailOrUserName);
            Assert.Equal("indalo123", authService.LastLoginRequest.Password);
            Assert.Equal(TestSessions.MemberSession, sessionService.CurrentSession);
            Assert.False(sessionService.LastRememberMe);
            Assert.Equal(1, loginSucceededCalls);
        });
    }

    [Fact]
    public async Task GoogleProviderError_ShowsControlledError()
    {
        var authService = new RecordingAuthService();
        var sessionService = new RecordingSessionService();
        Services.AddSingletonAuthDependencies(authService, sessionService, "google-client-id");
        JSInterop.Setup<bool>("indaloGoogleAuth.initializeButton", _ => true).SetResult(true);

        var cut = Render<LoginView>();

        await cut.InvokeAsync(() => cut.Instance.HandleGoogleSignInErrorAsync("popup_closed"));

        Assert.Contains("login_social_google_failed_error", cut.Markup);
    }

    [Fact]
    public void RendersForgotPasswordLink_AndOptionalStatusMessage()
    {
        var authService = new RecordingAuthService();
        var sessionService = new RecordingSessionService();
        Services.AddSingletonAuthDependencies(authService, sessionService, string.Empty);

        var cut = Render<LoginView>(parameters => parameters
            .Add(x => x.StatusMessageKey, "auth_reset_login_success"));

        Assert.Equal("/forgot-password", cut.Find(".login-form__options a").GetAttribute("href"));
        Assert.Contains("auth_reset_login_success", cut.Markup);
    }
}
