namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using IndaloaventurApp.Web.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class MyAccountPageTests : BunitContext
{
    [Fact]
    public void SignOut_ClearsLocalSession_ResetsGoogleState_AndNavigatesToLogin()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var memberProfileService = new RecordingMemberProfileService
        {
            GetMyProfileHandler = _ => Task.FromResult(
                ServiceResult<MemberProfile>.Success(
                    new MemberProfile(
                        Guid.NewGuid(),
                        "Usuario Demo",
                        "demo@indalo.es",
                        true,
                        null,
                        null,
                        "Socio")))
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IMemberProfileService>(memberProfileService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
        JSInterop.SetupVoid("indaloGoogleAuth.reset").SetVoidResult();

        var navigationManager = Services.GetRequiredService<NavigationManager>();
        var cut = Render<MyAccountPage>();

        cut.WaitForAssertion(() => Assert.Contains("Usuario Demo", cut.Markup));
        cut.Find("button.signout").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.False(sessionService.IsAuthenticated);
            Assert.Null(sessionService.CurrentSession);
            Assert.Equal(1, sessionService.SignOutCallCount);
            Assert.Single(JSInterop.Invocations, invocation => invocation.Identifier == "indaloGoogleAuth.reset");
            Assert.Equal("http://localhost/", navigationManager.Uri);
        });
    }
}
