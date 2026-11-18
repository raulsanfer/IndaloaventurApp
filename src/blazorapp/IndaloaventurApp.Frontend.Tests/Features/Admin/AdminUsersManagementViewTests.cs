namespace IndaloaventurApp.Frontend.Tests.Features.Admin;

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
            GetUsersHandler = (email, _, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(new[]
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
        var profile = CreateProfile(userId, "ana@club.es");
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (_, _, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(new[]
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
        cut.Find(".settings-users__actions button[type='button']").Click();

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
            GetUsersHandler = (email, _, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(
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

    [Fact]
    public void AdminUsersManagementView_DoesNotRenderInventoryPermissionEditor()
    {
        var user = new ManagedUserItem(Guid.NewGuid(), "ana@club.es", true, true, new[] { "Member", "InventoryManager" });
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (_, _, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(new[] { user }))
        };

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminUsersManagementView>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("settings-users-inventory-manager", cut.Markup);
            Assert.DoesNotContain("settings_users_inventory_manager_save", cut.Markup);
            Assert.Empty(cut.FindAll("input[type='checkbox']"));
        });
    }

    [Theory]
    [InlineData("", "ari", null, "ari")]
    [InlineData("", "  aRI  ", null, "aRI")]
    [InlineData("", "arc", null, "arc")]
    [InlineData("", "ont", null, "ont")]
    [InlineData("  CLUB.ES  ", "  Ana Marina  ", "club.es", "Ana Marina")]
    [InlineData("  CLUB.ES  ", "   ", "club.es", null)]
    [InlineData("  ", "  ", null, null)]
    public void Search_SubmitsEffectiveFiltersAndDisplaysOnlyServerResults(string email, string nombre, string? expectedEmail, string? expectedNombre)
    {
        var calls = new List<(string? Email, string? Nombre)>();
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (requestedEmail, requestedNombre, _) =>
            {
                calls.Add((requestedEmail, requestedNombre));
                return Task.FromResult(Users(calls.Count == 1 ? "inicial@club.es" : "resultado@club.es"));
            }
        };
        var cut = RenderUsers(service);
        Assert.Equal((null, null), Assert.Single(calls));

        cut.Find("#settings-users-email").Input(email);
        cut.Find("#settings-users-nombre").Input(nombre);
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(2, calls.Count);
            Assert.Equal((expectedEmail, expectedNombre), calls[1]);
            Assert.Contains("resultado@club.es", cut.Markup);
            Assert.DoesNotContain("inicial@club.es", cut.Markup);
        });
    }

    [Fact]
    public void Search_HasAccessibleFieldsAndNativeSubmitWithoutFullEmailValidation()
    {
        var cut = RenderUsers(new RecordingAdminUserManagementService());

        foreach (var id in new[] { "settings-users-email", "settings-users-nombre" })
        {
            Assert.NotNull(cut.Find($"label[for='{id}']"));
            Assert.Equal("text", cut.Find($"#{id}").GetAttribute("type"));
            Assert.False(cut.Find($"#{id}").HasAttribute("required"));
        }
        Assert.Equal("email", cut.Find("#settings-users-email").GetAttribute("inputmode"));
        Assert.Single(cut.FindAll("form button[type='submit']"));
        Assert.Equal("button", cut.Find("#settings-users-clear").GetAttribute("type"));
        Assert.Empty(cut.FindAll("[style]"));
    }

    [Fact]
    public void Search_ShowsMatchingNonMemberWithoutFilteringByMembershipOrEmail()
    {
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (_, nombre, _) => Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(
                nombre is null ? Array.Empty<ManagedUserItem>() : new[]
                {
                    new ManagedUserItem(Guid.NewGuid(), "cuenta@club.es", false, true, Array.Empty<string>())
                }))
        };
        var cut = RenderUsers(service);
        cut.Find("#settings-users-nombre").Input("Ana");
        cut.Find("form").Submit();

        Assert.Contains("cuenta@club.es", cut.Markup);
        Assert.Contains("settings_users_non_member_badge", cut.Markup);
        Assert.Contains("settings_users_create_member_file", cut.Markup);
    }

    [Theory]
    [InlineData("club.es", "", "match")]
    [InlineData("", "Ana", "match")]
    [InlineData("club.es", "Ana", "match")]
    [InlineData("", "Ana", "empty")]
    [InlineData("club.es", "Ana", "empty")]
    [InlineData("", "Ana", "error")]
    public void ClearFilters_EmptiesBothFieldsAndReloadsAllUsers(string email, string nombre, string outcome)
    {
        var calls = new List<(string? Email, string? Nombre)>();
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (requestedEmail, requestedNombre, _) =>
            {
                calls.Add((requestedEmail, requestedNombre));
                var result = requestedEmail is null && requestedNombre is null
                    ? Users("todos@club.es")
                    : outcome switch
                    {
                        "error" => FailedUsers("users.unavailable"),
                        "empty" => Users(),
                        _ => Users("filtrado@club.es")
                    };
                return Task.FromResult(result);
            }
        };
        var cut = RenderUsers(service);
        cut.Find("#settings-users-email").Input(email);
        cut.Find("#settings-users-nombre").Input(nombre);
        cut.Find("form").Submit();
        if (outcome == "empty")
        {
            Assert.Contains("settings_users_empty_filtered", cut.Markup);
        }
        else if (outcome == "error")
        {
            Assert.Contains("settings_users_search_error", cut.Markup);
        }

        cut.Find("#settings-users-clear").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(3, calls.Count);
            Assert.Equal((null, null), calls[2]);
            Assert.Equal("", cut.Find("#settings-users-email").GetAttribute("value"));
            Assert.Equal("", cut.Find("#settings-users-nombre").GetAttribute("value"));
            Assert.Contains("todos@club.es", cut.Markup);
            Assert.DoesNotContain("filtrado@club.es", cut.Markup);
            Assert.DoesNotContain("settings_users_search_error", cut.Markup);
            Assert.DoesNotContain("settings_users_empty_filtered", cut.Markup);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ClearFilters_UsesUnfilteredEmptyStateAndRecoversFromReloadFailure(bool failReload)
    {
        var calls = 0;
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (_, _, _) =>
            {
                calls++;
                return Task.FromResult(failReload && calls == 3 ? FailedUsers("users.unavailable") : Users());
            }
        };
        var cut = RenderUsers(service);
        Assert.Contains("settings_users_empty_all", cut.Markup);
        cut.Find("#settings-users-nombre").Input("Ana");
        cut.Find("form").Submit();
        Assert.Contains("settings_users_empty_filtered", cut.Markup);

        cut.Find("#settings-users-clear").Click();

        Assert.Equal("", cut.Find("#settings-users-nombre").GetAttribute("value"));
        Assert.DoesNotContain("settings_users_empty_filtered", cut.Markup);
        if (failReload)
        {
            Assert.Contains("settings_users_search_error", cut.Markup);
            Assert.DoesNotContain("settings_users_empty_all", cut.Markup);
            cut.Find("#settings-users-clear").Click();
            Assert.DoesNotContain("settings_users_search_error", cut.Markup);
        }
        Assert.Contains("settings_users_empty_all", cut.Markup);
    }

    [Theory]
    [InlineData("auth.session_invalid")]
    [InlineData("users.forbidden")]
    [InlineData("users.unavailable")]
    [InlineData("users.timeout")]
    [InlineData("users.invalid_payload")]
    public void Search_UsesSafeErrorFeedbackAndAllowsRetry(string code)
    {
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (_, nombre, _) => Task.FromResult(nombre == "Ana"
                ? FailedUsers(code)
                : Users("visible@club.es"))
        };
        var cut = RenderUsers(service);
        cut.Find("#settings-users-nombre").Input("Ana");
        cut.Find("form").Submit();

        Assert.Contains("settings_users_search_error", cut.Markup);
        Assert.DoesNotContain("Private server diagnostic", cut.Markup);
        Assert.DoesNotContain("visible@club.es", cut.Markup);
        Assert.DoesNotContain("settings_users_empty_filtered", cut.Markup);
        Assert.False(cut.Find("button[type='submit']").HasAttribute("disabled"));
        Assert.False(cut.Find("#settings-users-clear").HasAttribute("disabled"));

        cut.Find("#settings-users-nombre").Input("Marina");
        cut.Find("form").Submit();

        Assert.DoesNotContain("settings_users_search_error", cut.Markup);
        Assert.Contains("visible@club.es", cut.Markup);
    }

    [Fact]
    public async Task Search_PreventsOverlappingRequestsAndKeepsSubmittedFilterState()
    {
        var pending = new TaskCompletionSource<ServiceResult<IReadOnlyList<ManagedUserItem>>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = new List<(string? Email, string? Nombre)>();
        var service = new RecordingAdminUserManagementService
        {
            GetUsersHandler = (email, nombre, _) =>
            {
                calls.Add((email, nombre));
                return calls.Count == 2 ? pending.Task : Task.FromResult(Users());
            }
        };
        var cut = RenderUsers(service);
        cut.Find("#settings-users-nombre").Input("Ana");
        var search = cut.Find("form").SubmitAsync();
        cut.WaitForAssertion(() => Assert.Contains("settings_users_loading_filtered", cut.Markup));
        Assert.True(cut.Find("button[type='submit']").HasAttribute("disabled"));
        Assert.True(cut.Find("#settings-users-clear").HasAttribute("disabled"));

        cut.Find("#settings-users-nombre").Input("");
        await cut.Find("form").SubmitAsync();
        cut.Find("#settings-users-clear").Click();
        Assert.Equal(2, calls.Count);
        Assert.Equal((null, "Ana"), calls[1]);

        pending.SetResult(Users());
        await search;

        cut.WaitForAssertion(() => Assert.Contains("settings_users_empty_filtered", cut.Markup));
        cut.Find("#settings-users-clear").Click();
        Assert.Equal((null, null), calls[2]);
        Assert.Contains("settings_users_empty_all", cut.Markup);
    }

    private IRenderedComponent<AdminUsersManagementView> RenderUsers(RecordingAdminUserManagementService service)
    {
        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
        return Render<AdminUsersManagementView>();
    }

    private static ServiceResult<IReadOnlyList<ManagedUserItem>> Users(params string[] emails)
        => ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(emails
            .Select(email => new ManagedUserItem(Guid.NewGuid(), email, true, true, new[] { "Member" }))
            .ToArray());

    private static ServiceResult<IReadOnlyList<ManagedUserItem>> FailedUsers(string code)
        => ServiceResult<IReadOnlyList<ManagedUserItem>>.Failure(new ServiceError(code, "Private server diagnostic"));

    private static MemberSelfProfile CreateProfile(Guid userId, string email)
        => new(
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
            email,
            null,
            false,
            false,
            false);
}
