namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.Club;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class MemberMaterialReservationsListViewTests : BunitContext
{
    [Fact]
    public void List_ShowsOnlyCardSummaryFieldsInDescendingRequestOrderAndFloatingCreateAction()
    {
        var older = CreateReservation("Casco", new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc), "No debe mostrarse", new DateOnly(2026, 12, 3));
        var newer = CreateReservation("Piolet", new DateTime(2026, 10, 2, 9, 0, 0, DateTimeKind.Utc), "Tampoco debe mostrarse", new DateOnly(2026, 12, 4));
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([older, newer]))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationsListView>();
        cut.WaitForAssertion(() =>
        {
            Assert.True(cut.Markup.IndexOf("Piolet", StringComparison.Ordinal) < cut.Markup.IndexOf("Casco", StringComparison.Ordinal));
            Assert.Contains("member_material_reservations_request_date", cut.Markup);
            Assert.Contains("member_material_reservations_pickup_date", cut.Markup);
            Assert.Contains("member_material_reservations_status_requested", cut.Markup);
            Assert.DoesNotContain("No debe mostrarse", cut.Markup);
            Assert.DoesNotContain("Tampoco debe mostrarse", cut.Markup);
            Assert.Equal("/mi-club/reservas-material/nueva", cut.Find(".member-material-reservations__create-button").GetAttribute("href"));
        });
    }

    [Fact]
    public void List_HandlesExpiredSessionWithoutRetainingReservationData()
    {
        var session = new RecordingSessionService();
        session.SetSession(TestSessions.MemberSession);
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Failure(
                new ServiceError("auth.session_invalid", "Sesión no válida.")))
        };
        RegisterServices(service, session);

        var cut = Render<MemberMaterialReservationsListView>();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, session.SignOutCallCount);
            Assert.Equal("http://localhost/", Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().Uri);
            Assert.DoesNotContain("member-material-reservations__card", cut.Markup);
        });
    }

    [Fact]
    public void List_BlocksActionsAndHidesReservationDataWhenAccessIsForbidden()
    {
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Failure(
                new ServiceError("reservations.forbidden", "No autorizado.")))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationsListView>();
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("member_material_reservations_forbidden", cut.Markup);
            Assert.DoesNotContain("member-material-reservations__card", cut.Markup);
            Assert.DoesNotContain("member-material-reservations__create-button", cut.Markup);
        });
    }

    [Fact]
    public void List_RendersEmptyStateWhenTheMemberHasNoReservations()
    {
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([]))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationsListView>();
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("member_material_reservations_empty", cut.Markup);
            Assert.DoesNotContain("member-material-reservations__card", cut.Markup);
        });
    }

    [Fact]
    public void List_RendersSafeErrorStateWhenTheRequestFails()
    {
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Failure(
                new ServiceError("reservations.unavailable", "No se pueden cargar las reservas ahora.")))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationsListView>();
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("No se pueden cargar las reservas ahora.", cut.Markup);
            Assert.DoesNotContain("member-material-reservations__card", cut.Markup);
        });
    }

    [Fact]
    public void List_RendersLoadingStateUntilReservationsAreAvailable()
    {
        var completion = new TaskCompletionSource<ServiceResult<IReadOnlyList<MaterialReservation>>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => completion.Task
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationsListView>();
        Assert.Contains("member_material_reservations_loading", cut.Markup);

        completion.SetResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([]));
        cut.WaitForAssertion(() => Assert.Contains("member_material_reservations_empty", cut.Markup));
    }

    private void RegisterServices(RecordingMemberMaterialReservationService service, RecordingSessionService? session = null)
    {
        session ??= new RecordingSessionService();
        if (!session.IsAuthenticated)
        {
            session.SetSession(TestSessions.MemberSession);
        }

        Services.AddSingleton<IMemberMaterialReservationService>(service);
        Services.AddSingleton<ISessionService>(session);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
    }

    private static MaterialReservation CreateReservation(string material, DateTime requestedAtUtc, string notes, DateOnly actualReturnDate)
        => new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            42,
            material,
            2,
            requestedAtUtc,
            new DateOnly(2026, 11, 12),
            new DateOnly(2026, 11, 12),
            new DateOnly(2026, 11, 15),
            actualReturnDate,
            notes,
            "Información administrativa privada",
            MaterialReservationStatus.Requested,
            false,
            false,
            null,
            null);
}
