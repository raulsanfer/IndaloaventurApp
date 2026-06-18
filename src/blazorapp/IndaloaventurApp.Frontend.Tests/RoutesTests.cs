namespace IndaloaventurApp.Frontend.Tests;

using System.Threading.Tasks;
using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Auth;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.WordPress;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Resources;
using IndaloaventurApp.Web.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class RoutesTests : BunitContext
{
    [Fact]
    public void ProtectedRoute_RendersAfterInitialization_WhenSessionIsRestored()
    {
        var sessionService = new RecordingSessionService
        {
            IsInitialized = false
        };

        sessionService.EnsureInitializedHandler = () =>
        {
            sessionService.SetSession(TestSessions.MemberSession);
            sessionService.IsInitialized = true;
            return Task.CompletedTask;
        };

        RegisterRouteServices(sessionService);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/home");

        var cut = Render<Routes>();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, sessionService.EnsureInitializedCallCount);
            Assert.Contains("authenticated-shell", cut.Markup);
            Assert.Equal("http://localhost/home", Services.GetRequiredService<NavigationManager>().Uri);
        });
    }

    [Fact]
    public void ProtectedRoute_RedirectsToLoginAfterInitialization_WhenNoSessionCanBeRestored()
    {
        var sessionService = new RecordingSessionService
        {
            IsInitialized = false
        };

        sessionService.EnsureInitializedHandler = () =>
        {
            sessionService.IsInitialized = true;
            return Task.CompletedTask;
        };

        RegisterRouteServices(sessionService);
        Services.GetRequiredService<NavigationManager>().NavigateTo("/home");

        var cut = Render<Routes>();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, sessionService.EnsureInitializedCallCount);
            Assert.Equal("http://localhost/", Services.GetRequiredService<NavigationManager>().Uri);
            Assert.Contains("login-page", cut.Markup);
        });
    }

    private void RegisterRouteServices(ISessionService sessionService)
    {
        Services.AddSingleton(sessionService);
        Services.AddSingleton<IAuthService>(new RecordingAuthService());
        Services.AddSingleton<IWordPressPostService>(new RecordingWordPressPostService());
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
        Services.AddSingleton(new GoogleAuthOptions(string.Empty));
    }
}
