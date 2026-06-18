namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class AdminUsersManagementViewTests : BunitContext
{
    [Fact]
    public void AdminUsersManagementView_LoadsInitialUsersAndShowsMemberAndNonMemberActions()
    {
        var userId = Guid.NewGuid();
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (email, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(new[]
            {
                new ManagedUserItem(Guid.NewGuid(), "socio@club.es", true, true, new[] { "Member" }),
                new ManagedUserItem(userId, email ?? "nuevo@club.es", false, false, new[] { "Member" })
            }))
        };

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminUsersManagementView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("settings_users_edit_member_file", cut.Markup);
            Assert.Contains("settings_users_create_member_file", cut.Markup);
            Assert.Contains("settings_users_manage_federative_licenses", cut.Markup);
            Assert.Contains("nuevo@club.es", cut.Markup);
            Assert.Contains("settings_users_inactive_badge", cut.Markup);
            Assert.Contains("settings_users_active_badge", cut.Markup);
        });
    }

    [Fact]
    public void AdminUsersManagementView_NavigatesToMemberFile_WhenCreatingFromNonMember()
    {
        var userId = Guid.NewGuid();
        var profile = new MemberSelfProfile(
            userId,
            null,
            null,
            "Ana",
            "Montes",
            null,
            new DateOnly(1990, 4, 18),
            null,
            null,
            null,
            null,
            null,
            "ana@club.es",
            null,
            false,
            false,
            false);

        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(new[]
            {
                new ManagedUserItem(userId, "ana@club.es", false, true, new[] { "Member" })
            })),
            CreateMemberFileHandler = (requestedUserId, _, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile with
            {
                UserId = requestedUserId
            }))
        };

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminUsersManagementView>();
        var navigationManager = Services.GetRequiredService<NavigationManager>();

        cut.WaitForAssertion(() => Assert.Contains("settings_users_create_member_file", cut.Markup));

        cut.Find("button[type='button']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal($"http://localhost/configuracion/usuarios/{userId}/ficha", navigationManager.Uri);
        });
    }

    [Fact]
    public void AdminUsersManagementView_FiltersUsersByEmail()
    {
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (email, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(
                string.Equals(email, "ana@club.es", StringComparison.OrdinalIgnoreCase)
                    ? new[] { new ManagedUserItem(Guid.NewGuid(), "ana@club.es", true, true, new[] { "Member" }) }
                    : new[] { new ManagedUserItem(Guid.NewGuid(), "otro@club.es", false, true, new[] { "Member" }) }))
        };

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminUsersManagementView>();

        cut.Find("#settings-users-email").Input("ana@club.es");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("ana@club.es", cut.Markup);
            Assert.DoesNotContain("otro@club.es", cut.Markup);
        });
    }
}
