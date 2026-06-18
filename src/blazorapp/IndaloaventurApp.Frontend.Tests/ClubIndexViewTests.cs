namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.Club;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class ClubIndexViewTests : BunitContext
{
    [Fact]
    public void ClubIndexView_ShowsFederativeLicensesOption_ForMemberRole()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<ClubIndexView>();

        Assert.Contains("/mi-club/licencias-federativas", cut.Markup);
        Assert.Contains("mi_club_federative_licenses_title", cut.Markup);
    }

    [Fact]
    public void ClubIndexView_HidesFederativeLicensesOption_ForAdminRole()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Admin" }));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<ClubIndexView>();

        Assert.DoesNotContain("/mi-club/licencias-federativas", cut.Markup);
        Assert.DoesNotContain("mi_club_federative_licenses_title", cut.Markup);
    }

    [Fact]
    public void ClubIndexView_HidesFederativeLicensesOption_ForNonMemberAccess()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, Array.Empty<string>()));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<ClubIndexView>();

        Assert.DoesNotContain("/mi-club/licencias-federativas", cut.Markup);
        Assert.DoesNotContain("mi_club_federative_licenses_title", cut.Markup);
    }
}
