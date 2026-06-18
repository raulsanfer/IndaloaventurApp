namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class AdminMemberProfileViewTests : BunitContext
{
    [Fact]
    public void AdminMemberProfileView_RendersFormAndSubmitsSanitizedPayload()
    {
        var userId = Guid.NewGuid();
        UpdateMemberSelfProfileRequest? submittedRequest = null;
        var profile = new MemberSelfProfile(
            userId,
            2,
            "SecretarÃ­a",
            "Ana",
            "Montes",
            "12345678A",
            new DateOnly(1990, 4, 18),
            "Calle Sierra 5",
            "04001",
            "Almeria",
            "Almeria",
            "600123123",
            "ana@club.es",
            "Polen",
            true,
            false,
            true);

        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(
                new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Admin" }))),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile)),
            UpdateMemberFileHandler = (_, request, _) =>
            {
                submittedRequest = request;
                return Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile with
                {
                    Nombre = request.Nombre,
                    Email = request.Email
                }));
            }
        };

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<ICargoAdminService>(new RecordingCargoAdminService
        {
            GetCargosHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(
                new[]
                {
                    new CargoItem(2, "SecretarÃ­a"),
                    new CargoItem(5, "TesorerÃ­a")
                }))
        });
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("settings_admin_member_file_title", cut.Markup);
            Assert.Single(cut.FindAll("select#admin-member-cargo-id"));
            Assert.DoesNotContain("IsMember", cut.Markup);
            Assert.DoesNotContain("roles", cut.Markup, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("settings_users_active_badge", cut.Markup);
            Assert.Contains("settings_admin_member_file_cargo_empty_option", cut.Markup);
        });

        cut.Find("#admin-member-cargo-id").Change("5");
        cut.Find("#admin-member-nombre").Change("  Ana  ");
        cut.Find("#admin-member-email").Change("ANA@CLUB.ES");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(submittedRequest);
            Assert.Equal(5, submittedRequest!.CargoId);
            Assert.Equal("Ana", submittedRequest!.Nombre);
            Assert.Equal("ana@club.es", submittedRequest.Email);
            Assert.Contains("settings_admin_member_file_save_success", cut.Markup);
        });
    }

    [Fact]
    public void AdminMemberProfileView_AllowsClearingExistingCargo()
    {
        var userId = Guid.NewGuid();
        UpdateMemberSelfProfileRequest? submittedRequest = null;
        var profile = new MemberSelfProfile(
            userId,
            2,
            "SecretarÃƒÂ­a",
            "Ana",
            "Montes",
            "12345678A",
            new DateOnly(1990, 4, 18),
            "Calle Sierra 5",
            "04001",
            "Almeria",
            "Almeria",
            "600123123",
            "ana@club.es",
            "Polen",
            true,
            false,
            true);

        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(
                new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Admin" }))),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile)),
            UpdateMemberFileHandler = (_, request, _) =>
            {
                submittedRequest = request;
                return Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile with
                {
                    CargoId = request.CargoId,
                    CargoLabel = request.CargoId is null ? null : profile.CargoLabel
                }));
            }
        };

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<ICargoAdminService>(new RecordingCargoAdminService
        {
            GetCargosHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(
                new[]
                {
                    new CargoItem(2, "SecretarÃƒÂ­a"),
                    new CargoItem(5, "TesorerÃƒÂ­a")
                }))
        });
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));

        cut.WaitForAssertion(() => Assert.Contains("settings_admin_member_file_cargo_empty_option", cut.Markup));

        cut.Find("#admin-member-cargo-id").Change(string.Empty);
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(submittedRequest);
            Assert.Null(submittedRequest!.CargoId);
            Assert.Contains("settings_admin_member_file_save_success", cut.Markup);
        });
    }

    [Fact]
    public void AdminMemberProfileView_TogglesUserState()
    {
        var userId = Guid.NewGuid();
        var profile = new MemberSelfProfile(
            userId,
            null,
            null,
            "Ana",
            "Montes",
            "12345678A",
            new DateOnly(1990, 4, 18),
            "Calle Sierra 5",
            "04001",
            "Almeria",
            "Almeria",
            "600123123",
            "ana@club.es",
            "Polen",
            true,
            false,
            true);

        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(
                new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Admin" }))),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile)),
            DeactivateUserHandler = (_, _) => Task.FromResult(ServiceResult<bool>.Success(true))
        };

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<ICargoAdminService>(new RecordingCargoAdminService
        {
            GetCargosHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(Array.Empty<CargoItem>()))
        });
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));

        cut.WaitForAssertion(() => Assert.Contains("settings_admin_member_file_deactivate", cut.Markup));

        cut.Find("button[type='button']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("settings_admin_member_file_deactivate_success", cut.Markup);
            Assert.Contains("settings_admin_member_file_reactivate", cut.Markup);
            Assert.Contains("settings_users_inactive_badge", cut.Markup);
        });
    }
}
