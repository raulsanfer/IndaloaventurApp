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

public sealed class ClubMaterialReservationsPageTests : BunitContext
{
    [Theory]
    [InlineData(false, false, "http://localhost/")]
    [InlineData(true, false, "http://localhost/mi-club")]
    public void ReservationPages_RedirectUnauthorizedSessionsWithoutLoadingData(bool authenticated, bool isMember, string expectedUri)
    {
        var session = new RecordingSessionService();
        if (authenticated)
        {
            session.SetSession(new AuthSession("token", "Bearer", 3600, isMember, new[] { "Member" }));
        }

        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => throw new InvalidOperationException("Reservations must not load for an unauthorized session."),
            GetReservableMaterialsHandler = _ => throw new InvalidOperationException("Materials must not load for an unauthorized session."),
            GetMyReservationHandler = (_, _) => throw new InvalidOperationException("Detail must not load for an unauthorized session.")
        };
        Services.AddSingleton<ISessionService>(session);
        Services.AddSingleton<IMemberMaterialReservationService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<ClubMaterialReservationsPage>();

        Assert.Equal(expectedUri, Services.GetRequiredService<NavigationManager>().Uri);
        Assert.Equal(0, service.GetMyReservationsCallCount);
        Assert.Equal(0, service.GetReservableMaterialsCallCount);
        Assert.Equal(0, service.GetMyReservationCallCount);
    }

    [Fact]
    public void ReservationDetailPage_RedirectsAdminWithoutLoadingReservation()
    {
        var session = new RecordingSessionService();
        session.SetSession(new AuthSession("admin", "Bearer", 3600, true, new[] { "Admin" }));
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationHandler = (_, _) => throw new InvalidOperationException("Detail must not load for an administrator without Member role.")
        };
        Services.AddSingleton<ISessionService>(session);
        Services.AddSingleton<IMemberMaterialReservationService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<ClubMaterialReservationDetailPage>(parameters => parameters.Add(page => page.ReservationId, Guid.NewGuid()));

        Assert.Equal("http://localhost/mi-club", Services.GetRequiredService<NavigationManager>().Uri);
        Assert.Equal(0, service.GetMyReservationCallCount);
    }

    [Fact]
    public void ReservationCreatePage_RedirectsNonMemberWithoutLoadingMaterials()
    {
        var session = new RecordingSessionService();
        session.SetSession(new AuthSession("member", "Bearer", 3600, false, ["Member"]));
        var service = new RecordingMemberMaterialReservationService
        {
            GetReservableMaterialsHandler = _ => throw new InvalidOperationException("Materials must not load for a non-member session.")
        };
        Services.AddSingleton<ISessionService>(session);
        Services.AddSingleton<IMemberMaterialReservationService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        Render<ClubMaterialReservationCreatePage>();

        Assert.Equal("http://localhost/mi-club", Services.GetRequiredService<NavigationManager>().Uri);
        Assert.Equal(0, service.GetReservableMaterialsCallCount);
    }
}
