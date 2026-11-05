namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Resources;
using IndaloaventurApp.Web.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class SettingsMaterialReservationsPageTests : BunitContext
{
    /// <summary>Covers the direct-route denial for a session without the Admin role.</summary>
    [Fact]
    public void SettingsMaterialReservationsPage_RedirectsNonAdminWithoutLoadingReservations()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new AuthSession("member-token", "Bearer", 3600, true, new[] { "Member" }));
        var listCalls = 0;
        var reservationService = new RecordingInventoryReservationAdminService
        {
            GetReservationsHandler = _ =>
            {
                listCalls++;
                throw new InvalidOperationException("Reservations must not load for a non-admin session.");
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IInventoryReservationAdminService>(reservationService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<SettingsMaterialReservationsPage>();

        Assert.Equal("http://localhost/configuracion", Services.GetRequiredService<NavigationManager>().Uri);
        Assert.Equal(0, listCalls);
    }

    /// <summary>Covers the direct-route denial for an anonymous session.</summary>
    [Fact]
    public void SettingsMaterialReservationsPage_RedirectsAnonymousWithoutLoadingReservations()
    {
        var reservationService = new RecordingInventoryReservationAdminService
        {
            GetReservationsHandler = _ => throw new InvalidOperationException("Reservations must not load for an anonymous session.")
        };

        Services.AddSingleton<ISessionService>(new RecordingSessionService());
        Services.AddSingleton<IInventoryReservationAdminService>(reservationService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<SettingsMaterialReservationsPage>();

        Assert.Equal("http://localhost/", Services.GetRequiredService<NavigationManager>().Uri);
    }
}
