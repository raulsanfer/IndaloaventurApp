namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Resources;
using IndaloaventurApp.Web.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class SettingsInventoryPageTests : BunitContext
{
    [Fact]
    public void SettingsInventoryPage_RedirectsNonAdminWithoutLoadingInventory()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("member-token", "Bearer", 3600, true, new[] { "Member" }));
        var getCalls = 0;
        var inventoryService = new RecordingInventoryService
        {
            GetMaterialsHandler = _ =>
            {
                getCalls++;
                throw new InvalidOperationException("Inventory must not be loaded for a non-admin session.");
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IInventoryService>(inventoryService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<SettingsInventoryPage>();

        Assert.Equal("http://localhost/configuracion", Services.GetRequiredService<NavigationManager>().Uri);
        Assert.Equal(0, getCalls);
    }

    [Fact]
    public void SettingsInventoryPage_LoadsInventory_ForDelegatedMember()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("manager-token", "Bearer", 3600, true, new[] { "InventoryManager" }));
        var getCalls = 0;
        var inventoryService = new RecordingInventoryService
        {
            GetCategoriesHandler = _ =>
            {
                getCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<IndaloaventurApp.SharedUI.Models.Inventory.InventoryCategory>>.Success(Array.Empty<IndaloaventurApp.SharedUI.Models.Inventory.InventoryCategory>()));
            },
            GetStatusesHandler = _ =>
            {
                getCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<IndaloaventurApp.SharedUI.Models.Inventory.InventoryStatus>>.Success(Array.Empty<IndaloaventurApp.SharedUI.Models.Inventory.InventoryStatus>()));
            },
            GetMaterialsHandler = _ =>
            {
                getCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<IndaloaventurApp.SharedUI.Models.Inventory.InventoryMaterial>>.Success(Array.Empty<IndaloaventurApp.SharedUI.Models.Inventory.InventoryMaterial>()));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IInventoryService>(inventoryService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<SettingsInventoryPage>().WaitForAssertion(() => Assert.Equal(3, getCalls));

        Assert.Equal("http://localhost/", Services.GetRequiredService<NavigationManager>().Uri);
    }

    [Fact]
    public void SettingsInventoryPage_RedirectsNonMemberInventoryManagerWithoutLoadingInventory()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("manager-token", "Bearer", 3600, false, new[] { "InventoryManager" }));
        var getCalls = 0;
        var inventoryService = new RecordingInventoryService
        {
            GetMaterialsHandler = _ =>
            {
                getCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<IndaloaventurApp.SharedUI.Models.Inventory.InventoryMaterial>>.Success(Array.Empty<IndaloaventurApp.SharedUI.Models.Inventory.InventoryMaterial>()));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IInventoryService>(inventoryService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<SettingsInventoryPage>();

        Assert.Equal("http://localhost/configuracion", Services.GetRequiredService<NavigationManager>().Uri);
        Assert.Equal(0, getCalls);
    }
}
