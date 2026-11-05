namespace IndaloaventurApp.Frontend.Tests.Features.Navigation;

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
    public void ClubIndexView_ShowsMaterialReservationsImmediatelyAfterPhonebook_ForRealMember()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<ClubIndexView>();
        var phonebookPosition = cut.Markup.IndexOf("/mi-club/telefonos-interes", StringComparison.Ordinal);
        var reservationsPosition = cut.Markup.IndexOf("/mi-club/reservas-material", StringComparison.Ordinal);
        var licensesPosition = cut.Markup.IndexOf("/mi-club/licencias-federativas", StringComparison.Ordinal);

        Assert.True(phonebookPosition >= 0);
        Assert.True(reservationsPosition > phonebookPosition);
        Assert.True(licensesPosition > reservationsPosition);
    }

    [Theory]
    [InlineData(false, "Member")]
    [InlineData(true, "Admin")]
    public void ClubIndexView_HidesMaterialReservations_WhenRoleOrMembershipIsMissing(bool isMember, string role)
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, isMember, new[] { role }));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<ClubIndexView>();

        Assert.DoesNotContain("/mi-club/reservas-material", cut.Markup);
    }

    /// <summary>
    /// Covers scenario: club index view shows federative licenses option for member role.
    /// </summary>
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

    /// <summary>
    /// Covers scenario: club index view hides federative licenses option for admin role.
    /// </summary>
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

    /// <summary>
    /// Covers scenario: club index view hides federative licenses option for non member access.
    /// </summary>
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
