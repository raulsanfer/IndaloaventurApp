namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class MaterialReservationsManagementViewTests : BunitContext
{
    private static readonly Guid ReservationId = Guid.Parse("a06fbf98-339e-490a-b9a3-0d58a2e02f4b");

    /// <summary>Covers administrator list/detail rendering with application DaisyUI classes.</summary>
    [Fact]
    public void ReservationView_LoadsListAndDetailUsingDaisyUiLayout()
    {
        var reservation = CreateReservation(MaterialReservationStatus.Requested);
        var service = CreateService(reservation);
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));

        cut.Find($"#material-reservation-{ReservationId}").Click();
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Ana Socia", cut.Markup);
            Assert.Contains("ana@example.test", cut.Markup);
            Assert.Contains("material_reservations_status_requested", cut.Markup);
            Assert.NotEmpty(cut.FindAll(".material-reservations__layout"));
            Assert.NotEmpty(cut.FindAll(".card.shadow-sm"));
            Assert.NotEmpty(cut.FindAll(".badge.badge-warning"));
            Assert.NotEmpty(cut.FindAll(".btn.btn-primary"));
        });
    }

    /// <summary>Covers the read-only presentation of a final reservation state.</summary>
    [Fact]
    public void FinalReservation_DoesNotOfferTransitionActions()
    {
        var reservation = CreateReservation(MaterialReservationStatus.Returned, pickupDate: new DateOnly(2026, 11, 2));
        var service = CreateService(reservation);
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("material_reservations_final_state", cut.Markup);
            Assert.DoesNotContain("material_reservations_register_pickup</button>", cut.Markup);
            Assert.DoesNotContain("material_reservations_confirm_return</button>", cut.Markup);
            Assert.DoesNotContain("material_reservations_cancel</button>", cut.Markup);
        });
    }

    /// <summary>Covers pickup confirmation and remote-state refresh after a successful operation.</summary>
    [Fact]
    public void PickupConfirmation_SendsDateAndReloadsListAndDetailWithoutOptimisticState()
    {
        var currentReservation = CreateReservation(MaterialReservationStatus.Requested);
        var listCalls = 0;
        var detailCalls = 0;
        var service = new RecordingInventoryReservationAdminService
        {
            GetReservationsHandler = _ =>
            {
                listCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([currentReservation]));
            },
            GetReservationHandler = (_, _) =>
            {
                detailCalls++;
                return Task.FromResult(ServiceResult<MaterialReservation>.Success(currentReservation));
            },
            RegisterPickupHandler = (_, _, _) =>
            {
                currentReservation = currentReservation with
                {
                    Status = MaterialReservationStatus.PickedUp,
                    PickupDate = new DateOnly(2026, 11, 2)
                };
                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
        };
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(currentReservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();
        cut.WaitForAssertion(() => Assert.Contains("material_reservations_register_pickup", cut.Markup));

        FindButton(cut, "material_reservations_register_pickup").Click();
        cut.Find("#material-reservation-pickup-date").Change("2026-11-02");
        cut.Find(".material-reservations__modal form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(new DateOnly(2026, 11, 2), service.LastPickupRequest!.PickupDate);
            Assert.Equal(2, listCalls);
            Assert.Equal(2, detailCalls);
            Assert.Contains("material_reservations_status_picked_up", cut.Markup);
            Assert.Contains("material_reservations_action_updated", cut.Markup);
        });
    }

    /// <summary>Covers the state-dependent cancellation note requirement and normalization.</summary>
    [Fact]
    public void Cancellation_RequiresNoteForPickedUpReservationAndSendsTrimmedValue()
    {
        var reservation = CreateReservation(MaterialReservationStatus.PickedUp, pickupDate: new DateOnly(2026, 11, 2));
        var cancellationCalls = 0;
        var service = CreateService(
            reservation,
            cancelHandler: (_, _, _) =>
            {
                cancellationCalls++;
                return Task.FromResult(ServiceResult<bool>.Success(true));
            });
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();
        cut.WaitForAssertion(() => Assert.Contains("material_reservations_cancel", cut.Markup));

        FindButton(cut, "material_reservations_cancel").Click();
        cut.Find(".material-reservations__modal form").Submit();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(0, cancellationCalls);
            Assert.Contains("material_reservations_required_field", cut.Markup);
        });

        cut.Find("#material-reservation-cancellation-note").Change("  Material dañado  ");
        cut.Find(".material-reservations__modal form").Submit();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, cancellationCalls);
            Assert.Equal("Material dañado", service.LastCancellationRequest!.AdministrationNote);
        });
    }

    /// <summary>Covers local return-date validation against the effective pickup date.</summary>
    [Fact]
    public void ReturnConfirmation_BlocksDateBeforePickupAndSendsValidDate()
    {
        var reservation = CreateReservation(MaterialReservationStatus.PickedUp, pickupDate: new DateOnly(2026, 11, 2));
        var returnCalls = 0;
        var service = CreateService(
            reservation,
            returnHandler: (_, _, _) =>
            {
                returnCalls++;
                return Task.FromResult(ServiceResult<bool>.Success(true));
            });
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();
        cut.WaitForAssertion(() => Assert.Contains("material_reservations_confirm_return", cut.Markup));

        FindButton(cut, "material_reservations_confirm_return").Click();
        cut.Find("#material-reservation-return-date").Change("2026-11-01");
        cut.Find(".material-reservations__modal form").Submit();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(0, returnCalls);
            Assert.Contains("material_reservations_return_before_pickup", cut.Markup);
        });

        cut.Find("#material-reservation-return-date").Change("2026-11-08");
        cut.Find(".material-reservations__modal form").Submit();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, returnCalls);
            Assert.Equal(new DateOnly(2026, 11, 8), service.LastReturnConfirmationRequest!.ActualReturnDate);
        });
    }

    /// <summary>Covers removal of personally identifiable detail after a missing reservation response.</summary>
    [Fact]
    public void DetailNotFound_ClearsMemberDataAndReloadsList()
    {
        var reservation = CreateReservation(MaterialReservationStatus.Requested);
        var listCalls = 0;
        var service = new RecordingInventoryReservationAdminService
        {
            GetReservationsHandler = _ =>
            {
                listCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([reservation]));
            },
            GetReservationHandler = (_, _) => Task.FromResult(ServiceResult<MaterialReservation>.Failure(
                new ServiceError("reservations.not_found", "La reserva ya no existe.")))
        };
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("material_reservations_not_found", cut.Markup);
            Assert.DoesNotContain("Ana Socia", cut.Markup);
            Assert.DoesNotContain("ana@example.test", cut.Markup);
            Assert.Equal(2, listCalls);
        });
    }

    /// <summary>Covers action blocking after the API denies administrator permission.</summary>
    [Fact]
    public void ForbiddenAction_HidesFurtherTransitionsAndExplainsPermissionFailure()
    {
        var reservation = CreateReservation(MaterialReservationStatus.Requested);
        var service = CreateService(
            reservation,
            pickupHandler: (_, _, _) => Task.FromResult(ServiceResult<bool>.Failure(
                new ServiceError("reservations.forbidden", "No autorizado."))));
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();
        cut.WaitForAssertion(() => Assert.Contains("material_reservations_register_pickup", cut.Markup));
        FindButton(cut, "material_reservations_register_pickup").Click();
        cut.Find("#material-reservation-pickup-date").Change("2026-11-02");
        cut.Find(".material-reservations__modal form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("material_reservations_forbidden", cut.Markup);
            Assert.DoesNotContain(cut.FindAll("button"), button => button.TextContent.Contains("material_reservations_register_pickup", StringComparison.Ordinal));
        });
    }

    /// <summary>Covers session clearing and navigation after an expired administrator session.</summary>
    [Fact]
    public void UnauthorizedAction_ClearsSessionAndSelectedMemberData()
    {
        var reservation = CreateReservation(MaterialReservationStatus.Requested);
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);
        var service = CreateService(
            reservation,
            pickupHandler: (_, _, _) => Task.FromResult(ServiceResult<bool>.Failure(
                new ServiceError("auth.session_invalid", "La sesión ya no es válida."))));
        RegisterServices(service, sessionService);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();
        cut.WaitForAssertion(() => Assert.Contains("Ana Socia", cut.Markup));
        FindButton(cut, "material_reservations_register_pickup").Click();
        cut.Find("#material-reservation-pickup-date").Change("2026-11-02");
        cut.Find(".material-reservations__modal form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, sessionService.SignOutCallCount);
            Assert.Equal("http://localhost/", Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().Uri);
            Assert.DoesNotContain("Ana Socia", cut.Markup);
            Assert.DoesNotContain("ana@example.test", cut.Markup);
        });
    }

    /// <summary>Covers preservation of a decision on validation or concurrency failures.</summary>
    [Fact]
    public void ValidationAndConflictFailures_PreserveDecisionAndRefreshBeforeRetry()
    {
        var reservation = CreateReservation(MaterialReservationStatus.Requested);
        var pickupCalls = 0;
        var listCalls = 0;
        var detailCalls = 0;
        var service = new RecordingInventoryReservationAdminService
        {
            GetReservationsHandler = _ =>
            {
                listCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([reservation]));
            },
            GetReservationHandler = (_, _) =>
            {
                detailCalls++;
                return Task.FromResult(ServiceResult<MaterialReservation>.Success(reservation));
            },
            RegisterPickupHandler = (_, _, _) =>
            {
                pickupCalls++;
                return Task.FromResult(pickupCalls == 1
                    ? ServiceResult<bool>.Failure(new ServiceError(
                        "reservations.validation",
                        "La fecha no es válida.",
                        [new ServiceValidationError("fechaRetirada", "Corrige la fecha.")]))
                    : ServiceResult<bool>.Failure(new ServiceError("reservations.conflict", "La reserva cambió de estado.")));
            }
        };
        RegisterServices(service);

        var cut = Render<MaterialReservationsManagementView>();
        cut.WaitForAssertion(() => Assert.Contains(reservation.MaterialDescription, cut.Markup));
        cut.Find($"#material-reservation-{ReservationId}").Click();
        cut.WaitForAssertion(() => Assert.Contains("material_reservations_register_pickup", cut.Markup));
        FindButton(cut, "material_reservations_register_pickup").Click();
        cut.Find("#material-reservation-pickup-date").Change("2026-11-02");
        cut.Find(".material-reservations__modal form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, pickupCalls);
            Assert.Contains("Corrige la fecha.", cut.Markup);
            Assert.Equal("2026-11-02", cut.Find("#material-reservation-pickup-date").GetAttribute("value"));
        });

        cut.Find(".material-reservations__modal form").Submit();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(2, pickupCalls);
            Assert.Contains("La reserva cambió de estado.", cut.Markup);
            Assert.Equal(2, listCalls);
            Assert.Equal(2, detailCalls);
            Assert.Equal("2026-11-02", cut.Find("#material-reservation-pickup-date").GetAttribute("value"));
        });
    }

    private void RegisterServices(RecordingInventoryReservationAdminService service, RecordingSessionService? sessionService = null)
    {
        sessionService ??= new RecordingSessionService();
        if (!sessionService.IsAuthenticated)
        {
            sessionService.SetSession(TestSessions.AdminSession);
        }

        Services.AddSingleton<IInventoryReservationAdminService>(service);
        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
    }

    private static RecordingInventoryReservationAdminService CreateService(
        MaterialReservation reservation,
        Func<Guid, RegisterAdminMaterialReservationPickupRequest, CancellationToken, Task<ServiceResult<bool>>>? pickupHandler = null,
        Func<Guid, CancelAdminMaterialReservationRequest, CancellationToken, Task<ServiceResult<bool>>>? cancelHandler = null,
        Func<Guid, ConfirmAdminMaterialReservationReturnRequest, CancellationToken, Task<ServiceResult<bool>>>? returnHandler = null)
        => new()
        {
            GetReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([reservation])),
            GetReservationHandler = (_, _) => Task.FromResult(ServiceResult<MaterialReservation>.Success(reservation)),
            RegisterPickupHandler = pickupHandler,
            CancelReservationHandler = cancelHandler,
            ConfirmReturnHandler = returnHandler
        };

    private static MaterialReservation CreateReservation(MaterialReservationStatus status, DateOnly? pickupDate = null)
        => new(
            ReservationId,
            Guid.Parse("e17c2be2-c50f-4c37-bfca-8019c5fc3da9"),
            42,
            "Piolet técnico",
            2,
            new DateTime(2026, 10, 1, 9, 30, 0, DateTimeKind.Utc),
            new DateOnly(2026, 11, 1),
            pickupDate,
            new DateOnly(2026, 11, 8),
            status == MaterialReservationStatus.Returned ? new DateOnly(2026, 11, 8) : null,
            "Usaré el material en una salida formativa.",
            null,
            status,
            status == MaterialReservationStatus.Returned,
            status == MaterialReservationStatus.Returned,
            new MaterialReservationMember(Guid.Parse("e17c2be2-c50f-4c37-bfca-8019c5fc3da9"), "Ana Socia", "ana@example.test"),
            new MaterialReservationAvailability(42, 5, 2, 3, true));

    private static AngleSharp.Dom.IElement FindButton(IRenderedComponent<MaterialReservationsManagementView> cut, string text)
        => cut.FindAll("button").Single(button => button.TextContent.Contains(text, StringComparison.Ordinal));
}
