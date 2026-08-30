namespace IndaloaventurApp.Frontend.Tests.Features.Navigation;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.Shell;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class BottomNavTests : BunitContext
{
    /// <summary>
    /// Covers scenario: bottom nav shows club item for members.
    /// </summary>
    [Fact]
    public void BottomNav_ShowsClubItem_ForMembers()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<BottomNav>();

        Assert.Contains("/mi-club", cut.Markup);
        Assert.Contains("shell_nav_club", cut.Markup);
    }

    /// <summary>
    /// Covers scenario: bottom nav hides club item for non members.
    /// </summary>
    [Fact]
    public void BottomNav_HidesClubItem_ForNonMembers()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<BottomNav>();

        Assert.DoesNotContain("/mi-club", cut.Markup);
        Assert.DoesNotContain("shell_nav_club", cut.Markup);
    }
}
