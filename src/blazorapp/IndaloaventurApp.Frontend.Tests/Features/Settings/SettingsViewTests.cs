namespace IndaloaventurApp.Frontend.Tests.Features.Settings;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.MyAccount;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class SettingsViewTests : BunitContext
{
    /// <summary>
    /// Covers scenario: settings view hides administration panel for non admin.
    /// </summary>
    [Fact]
    public void SettingsView_HidesAdministrationPanel_ForNonAdmin()
    {
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SettingsView>(parameters => parameters.Add(x => x.IsAdmin, false));

        Assert.Contains("shell_nav_my_account", cut.Markup);
        Assert.Contains("settings_title", cut.Markup);
        Assert.DoesNotContain("settings_summary", cut.Markup);
        Assert.DoesNotContain("/configuracion/cargos", cut.Markup);
        Assert.DoesNotContain("/configuracion/usuarios", cut.Markup);
        Assert.DoesNotContain("/configuracion/licencias-federativas", cut.Markup);
        Assert.DoesNotContain("/configuracion/signals", cut.Markup);
        Assert.DoesNotContain("settings_admin_title", cut.Markup);
    }

    /// <summary>
    /// Covers scenario: settings view shows administration panel for admin.
    /// </summary>
    [Fact]
    public void SettingsView_ShowsAdministrationPanel_ForAdmin()
    {
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SettingsView>(parameters => parameters.Add(x => x.IsAdmin, true));

        Assert.Contains("shell_nav_my_account", cut.Markup);
        Assert.Contains("settings_title", cut.Markup);
        Assert.Contains("settings_admin_title", cut.Markup);
        Assert.Contains("fieldset", cut.Markup);
        Assert.Contains("/configuracion/cargos", cut.Markup);
        Assert.Contains("/configuracion/usuarios", cut.Markup);
        Assert.Contains("/configuracion/licencias-federativas", cut.Markup);
        Assert.Contains("/configuracion/signals", cut.Markup);
        Assert.DoesNotContain("settings_summary", cut.Markup);
        Assert.DoesNotContain("settings_admin_description", cut.Markup);
        Assert.DoesNotContain("settings_admin_cargos_description", cut.Markup);
    }

    /// <summary>
    /// Covers scenario: my account view renders operational settings link.
    /// </summary>
    [Fact]
    public void MyAccountView_RendersOperationalSettingsLink()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var profileService = new RecordingMemberProfileService
        {
            GetMyProfileHandler = _ => Task.FromResult(ServiceResult<MemberProfile>.Success(
                new MemberProfile(Guid.NewGuid(), "Admin Demo", "admin@club.es", true, 1, "Presidencia", "SOCIO PREMIUM")))
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IMemberProfileService>(profileService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<MyAccountView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("href=\"/configuracion\"", cut.Markup);
            Assert.Contains("href=\"/mi-cuenta/ficha-socio\"", cut.Markup);
        });
    }

    /// <summary>
    /// Covers scenario: my account view shows join member link for member role without membership.
    /// </summary>
    [Fact]
    public void MyAccountView_ShowsJoinMemberLink_ForMemberRoleWithoutMembership()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Member" }));

        var profileService = new RecordingMemberProfileService
        {
            GetMyProfileHandler = _ => Task.FromResult(ServiceResult<MemberProfile>.Success(null!))
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IMemberProfileService>(profileService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<MyAccountView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("mi_cuenta_link_join_member", cut.Markup);
            Assert.Contains("https://indaloaventura.com/hazte-socio/", cut.Markup);
            Assert.Contains("target=\"_blank\"", cut.Markup);
            Assert.Contains("rel=\"noopener noreferrer\"", cut.Markup);
        });
    }

    /// <summary>
    /// Covers scenario: my account view hides join member link for actual members.
    /// </summary>
    [Fact]
    public void MyAccountView_HidesJoinMemberLink_ForActualMembers()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, true, new[] { "Member" }));

        var profileService = new RecordingMemberProfileService
        {
            GetMyProfileHandler = _ => Task.FromResult(ServiceResult<MemberProfile>.Success(
                new MemberProfile(Guid.NewGuid(), "Socia Demo", "socia@club.es", true, null, null, "SOCIO PREMIUM")))
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IMemberProfileService>(profileService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<MyAccountView>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("mi_cuenta_link_join_member", cut.Markup);
            Assert.DoesNotContain("https://indaloaventura.com/hazte-socio/", cut.Markup);
        });
    }

    /// <summary>
    /// Covers scenario: my account view does not show error for non member without profile.
    /// </summary>
    [Fact]
    public void MyAccountView_DoesNotShowError_ForNonMemberWithoutProfile()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("token", "Bearer", 3600, false, new[] { "Member" }));

        var profileService = new RecordingMemberProfileService
        {
            GetMyProfileHandler = _ => Task.FromResult(ServiceResult<MemberProfile>.Success(null!))
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IMemberProfileService>(profileService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<MyAccountView>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("mi_cuenta_load_error", cut.Markup);
            Assert.Contains("href=\"/configuracion\"", cut.Markup);
        });
    }
}
