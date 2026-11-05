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

public sealed class MemberMaterialReservationDetailViewTests : BunitContext
{
    private static readonly Guid ReservationId = Guid.Parse("2728be8a-b0fa-47c0-bc25-92de1b0a9dc1");

    [Fact]
    public void Detail_OffersOnlyStateAllowedActionsAndNeverAnEditAction()
    {
        var requested = CreateReservation(MaterialReservationStatus.Requested);
        var requestedService = CreateService(requested);
        RegisterServices(requestedService);
        var requestedCut = Render<MemberMaterialReservationDetailView>(parameters => parameters.Add(view => view.ReservationId, ReservationId));
        requestedCut.WaitForAssertion(() =>
        {
            Assert.Contains("member_material_reservations_cancel", requestedCut.Markup);
            Assert.DoesNotContain("member_material_reservations_confirm_return</button>", requestedCut.Markup);
            Assert.DoesNotContain("Editar", requestedCut.Markup, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Theory]
    [InlineData(MaterialReservationStatus.Returned)]
    [InlineData(MaterialReservationStatus.Cancelled)]
    public void Detail_FinalStatesAreReadOnly(MaterialReservationStatus status)
    {
        var service = CreateService(CreateReservation(status));
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationDetailView>(parameters => parameters.Add(view => view.ReservationId, ReservationId));
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("member_material_reservations_read_only", cut.Markup);
            Assert.DoesNotContain("member_material_reservations_cancel</button>", cut.Markup);
            Assert.DoesNotContain("member_material_reservations_confirm_return</button>", cut.Markup);
        });
    }

    [Fact]
    public void Detail_RefreshesRemoteStateAfterCancellationWithoutOptimisticUpdate()
    {
        var current = CreateReservation(MaterialReservationStatus.Requested);
        var listCalls = 0;
        var detailCalls = 0;
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ =>
            {
                listCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([current]));
            },
            GetMyReservationHandler = (_, _) =>
            {
                detailCalls++;
                return Task.FromResult(ServiceResult<MaterialReservation>.Success(current));
            },
            CancelReservationHandler = (_, _) =>
            {
                current = current with { Status = MaterialReservationStatus.Cancelled };
                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationDetailView>(parameters => parameters.Add(view => view.ReservationId, ReservationId));
        cut.WaitForAssertion(() => Assert.Contains("member_material_reservations_cancel", cut.Markup));
        FindButton(cut, "member_material_reservations_cancel").Click();
        cut.WaitForAssertion(() => Assert.Contains("member_material_reservations_confirm_cancellation", cut.Markup));
        FindButton(cut, "member_material_reservations_confirm_cancellation").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, service.CancelCallCount);
            Assert.Equal(1, listCalls);
            Assert.Equal(2, detailCalls);
            Assert.Contains("member_material_reservations_status_cancelled", cut.Markup);
            Assert.DoesNotContain("member_material_reservations_cancel</button>", cut.Markup);
        });
    }

    [Fact]
    public void Detail_ConfirmsReturnOnlyForPickedUpReservation()
    {
        var reservation = CreateReservation(MaterialReservationStatus.PickedUp);
        var service = CreateService(reservation, confirmHandler: (_, _) => Task.FromResult(ServiceResult<bool>.Success(true)));
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationDetailView>(parameters => parameters.Add(view => view.ReservationId, ReservationId));
        cut.WaitForAssertion(() => Assert.Contains("member_material_reservations_confirm_return", cut.Markup));
        Assert.DoesNotContain("member_material_reservations_cancel</button>", cut.Markup);
        FindButton(cut, "member_material_reservations_confirm_return").Click();
        cut.WaitForAssertion(() => Assert.Contains("member_material_reservations_return_confirmation", cut.Markup));
        cut.Find(".member-material-reservations__modal .btn-primary").Click();

        cut.WaitForAssertion(() => Assert.Equal(1, service.ConfirmReturnCallCount));
    }

    [Fact]
    public void Detail_RefreshesRemoteStateAndClosesStaleConfirmationAfterConflict()
    {
        var current = CreateReservation(MaterialReservationStatus.Requested);
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([current])),
            GetMyReservationHandler = (_, _) => Task.FromResult(ServiceResult<MaterialReservation>.Success(current)),
            CancelReservationHandler = (_, _) =>
            {
                current = current with { Status = MaterialReservationStatus.PickedUp };
                return Task.FromResult(ServiceResult<bool>.Failure(
                    new ServiceError("reservations.conflict", "La reserva ha cambiado de estado.")));
            }
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationDetailView>(parameters => parameters.Add(view => view.ReservationId, ReservationId));
        cut.WaitForAssertion(() => Assert.Contains("member_material_reservations_cancel", cut.Markup));
        FindButton(cut, "member_material_reservations_cancel").Click();
        FindButton(cut, "member_material_reservations_confirm_cancellation").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, service.CancelCallCount);
            Assert.Contains("La reserva ha cambiado de estado.", cut.Markup);
            Assert.Contains("member_material_reservations_status_picked_up", cut.Markup);
            Assert.Contains("member_material_reservations_confirm_return", cut.Markup);
            Assert.DoesNotContain("member-material-reservations__modal", cut.Markup);
        });
    }

    [Fact]
    public void Detail_NotFoundNavigatesToListAndDoesNotRenderOldReservation()
    {
        var service = new RecordingMemberMaterialReservationService
        {
            GetMyReservationHandler = (_, _) => Task.FromResult(ServiceResult<MaterialReservation>.Failure(
                new ServiceError("reservations.not_found", "No disponible.")))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationDetailView>(parameters => parameters.Add(view => view.ReservationId, ReservationId));
        cut.WaitForAssertion(() =>
        {
            Assert.Equal("http://localhost/mi-club/reservas-material", Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().Uri);
            Assert.DoesNotContain("Piolet técnico", cut.Markup);
        });
    }

    private void RegisterServices(RecordingMemberMaterialReservationService service)
    {
        var session = new RecordingSessionService();
        session.SetSession(TestSessions.MemberSession);
        Services.AddSingleton<IMemberMaterialReservationService>(service);
        Services.AddSingleton<ISessionService>(session);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
    }

    private static RecordingMemberMaterialReservationService CreateService(
        MaterialReservation reservation,
        Func<Guid, CancellationToken, Task<ServiceResult<bool>>>? confirmHandler = null)
        => new()
        {
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([reservation])),
            GetMyReservationHandler = (_, _) => Task.FromResult(ServiceResult<MaterialReservation>.Success(reservation)),
            ConfirmReturnHandler = confirmHandler
        };

    private static MaterialReservation CreateReservation(MaterialReservationStatus status)
        => new(
            ReservationId,
            Guid.Parse("756f92e6-19a5-4334-965b-586517b2fc15"),
            42,
            "Piolet técnico",
            2,
            new DateTime(2026, 10, 1, 9, 30, 0, DateTimeKind.Utc),
            new DateOnly(2026, 11, 12),
            status == MaterialReservationStatus.PickedUp ? new DateOnly(2026, 11, 12) : null,
            new DateOnly(2026, 11, 15),
            null,
            "Usaré el material en una salida formativa.",
            null,
            status,
            false,
            false,
            null,
            null);

    private static AngleSharp.Dom.IElement FindButton(IRenderedComponent<MemberMaterialReservationDetailView> cut, string text)
        => cut.FindAll("button").Single(button => button.TextContent.Contains(text, StringComparison.Ordinal));
}
