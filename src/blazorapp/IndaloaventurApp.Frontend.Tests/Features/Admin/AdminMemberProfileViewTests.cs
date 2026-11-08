namespace IndaloaventurApp.Frontend.Tests.Features.Admin;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class AdminMemberProfileViewTests : BunitContext
{
    /// <summary>
    /// Covers scenario: admin member profile view renders form and submits sanitized payload.
    /// </summary>
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
        Services.AddSingleton<ISessionService>(new RecordingSessionService());
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
            Assert.Contains($"/configuracion/usuarios/{userId}/contrasena", cut.Markup);
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

    /// <summary>
    /// Covers scenario: admin member profile view allows clearing existing cargo.
    /// </summary>
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
        Services.AddSingleton<ISessionService>(new RecordingSessionService());
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

    /// <summary>
    /// Covers scenario: admin member profile view toggles user state.
    /// </summary>
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
        Services.AddSingleton<ISessionService>(new RecordingSessionService());
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

    [Fact]
    public void AdminMemberProfileView_GrantsInventoryManagerAndPreservesOtherRoles()
    {
        var userId = Guid.NewGuid();
        var user = new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Member", "CustomRole" });
        UpdateManagedUserRequest? updateRequest = null;
        var getUserCallCount = 0;
        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) =>
            {
                getUserCallCount++;
                return Task.FromResult(ServiceResult<ManagedUserItem>.Success(user));
            },
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(CreateProfile(userId, user.Email))),
            UpdateUserHandler = (_, request, _) =>
            {
                updateRequest = request;
                user = user with { Roles = request.Roles };
                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
        };
        var sessionService = CreateAdminSession(Guid.NewGuid());
        RegisterDetailServices(service, sessionService);

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find("#admin-member-inventory-manager")));

        cut.Find("#admin-member-inventory-manager").Change(true);
        cut.Find("#admin-member-inventory-manager-save").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(updateRequest);
            Assert.Equal(new[] { "Member", "CustomRole", "InventoryManager" }, updateRequest!.Roles);
            Assert.Equal(2, getUserCallCount);
            Assert.Contains("settings_users_inventory_manager_save_success", cut.Markup);
            Assert.True(cut.Find("#admin-member-inventory-manager").HasAttribute("checked"));
        });
    }

    [Fact]
    public void AdminMemberProfileView_RemovesInventoryManagerAndPreservesOtherRoles()
    {
        var userId = Guid.NewGuid();
        var user = new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Member", "InventoryManager", "CustomRole" });
        UpdateManagedUserRequest? updateRequest = null;
        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(user)),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(CreateProfile(userId, user.Email))),
            UpdateUserHandler = (_, request, _) =>
            {
                updateRequest = request;
                user = user with { Roles = request.Roles };
                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
        };
        RegisterDetailServices(service, CreateAdminSession(Guid.NewGuid()));

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));
        cut.WaitForAssertion(() => Assert.True(cut.Find("#admin-member-inventory-manager").HasAttribute("checked")));

        cut.Find("#admin-member-inventory-manager").Change(false);
        cut.Find("#admin-member-inventory-manager-save").Click();

        cut.WaitForAssertion(() => Assert.Equal(new[] { "Member", "CustomRole" }, updateRequest!.Roles));
    }

    [Fact]
    public void AdminMemberProfileView_SignsOutWhenAdministratorUpdatesOwnInventoryRole()
    {
        var userId = Guid.NewGuid();
        var user = new ManagedUserItem(userId, "admin@club.es", true, true, new[] { "Admin" });
        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(user)),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(CreateProfile(userId, user.Email))),
            UpdateUserHandler = (_, _, _) => Task.FromResult(ServiceResult<bool>.Success(true))
        };
        var sessionService = CreateAdminSession(userId);
        RegisterDetailServices(service, sessionService);

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find("#admin-member-inventory-manager")));

        cut.Find("#admin-member-inventory-manager").Change(true);
        cut.Find("#admin-member-inventory-manager-save").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, sessionService.SignOutCallCount);
            Assert.Equal("http://localhost/", Services.GetRequiredService<NavigationManager>().Uri);
        });
    }

    [Theory]
    [InlineData("auth.session_invalid", true)]
    [InlineData("users.forbidden", false)]
    public void AdminMemberProfileView_HandlesInventoryPermissionAuthorizationFailures(string errorCode, bool shouldSignOut)
    {
        var userId = Guid.NewGuid();
        var user = new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Member" });
        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(user)),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(CreateProfile(userId, user.Email))),
            UpdateUserHandler = (_, _, _) => Task.FromResult(ServiceResult<bool>.Failure(new ServiceError(errorCode, "No autorizado.")))
        };
        var sessionService = CreateAdminSession(Guid.NewGuid());
        RegisterDetailServices(service, sessionService);

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find("#admin-member-inventory-manager")));

        cut.Find("#admin-member-inventory-manager").Change(true);
        cut.Find("#admin-member-inventory-manager-save").Click();

        cut.WaitForAssertion(() => Assert.Equal(shouldSignOut ? 1 : 0, sessionService.SignOutCallCount));
        if (shouldSignOut)
        {
            Assert.Equal("http://localhost/", Services.GetRequiredService<NavigationManager>().Uri);
        }
        else
        {
            Assert.Contains("settings_users_inventory_manager_forbidden", cut.Markup);
            Assert.False(cut.Find("#admin-member-inventory-manager").HasAttribute("checked"));
        }
    }

    [Fact]
    public void AdminMemberProfileView_RefreshesManagedUserBeforeLaterInventoryPermissionSave()
    {
        var userId = Guid.NewGuid();
        var user = new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Member" });
        UpdateManagedUserRequest? updateRequest = null;
        var service = new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(user)),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(CreateProfile(userId, user.Email))),
            UpdateMemberFileHandler = (_, request, _) =>
            {
                var updatedEmail = request.Email ?? user.Email;
                user = user with { Email = updatedEmail };
                return Task.FromResult(ServiceResult<MemberSelfProfile>.Success(CreateProfile(userId, updatedEmail)));
            },
            UpdateUserHandler = (_, request, _) =>
            {
                updateRequest = request;
                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
        };
        RegisterDetailServices(service, CreateAdminSession(Guid.NewGuid()));

        var cut = Render<AdminMemberProfileView>(parameters => parameters.Add(x => x.UserId, userId));
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find("#admin-member-email")));

        cut.Find("#admin-member-email").Change("nueva@club.es");
        cut.Find("form").Submit();
        cut.WaitForAssertion(() => Assert.Contains("settings_admin_member_file_save_success", cut.Markup));

        cut.Find("#admin-member-inventory-manager").Change(true);
        cut.Find("#admin-member-inventory-manager-save").Click();

        cut.WaitForAssertion(() => Assert.Equal("nueva@club.es", updateRequest!.Email));
    }

    private void RegisterDetailServices(RecordingAdminUserManagementService service, RecordingSessionService sessionService)
    {
        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<ICargoAdminService>(new RecordingCargoAdminService
        {
            GetCargosHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(Array.Empty<CargoItem>()))
        });
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
    }

    private static RecordingSessionService CreateAdminSession(Guid userId)
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("admin-token", "Bearer", 3600, true, new[] { "Admin" }, userId));
        return sessionService;
    }

    private static MemberSelfProfile CreateProfile(Guid userId, string email)
        => new(
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
            email,
            null,
            false,
            false,
            false);
}
