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

public sealed class MemberMaterialReservationCreateViewTests : BunitContext
{
    [Fact]
    public void CreateForm_ExcludesInactiveMaterialAndBlocksInvalidReturnDate()
    {
        var service = new RecordingMemberMaterialReservationService
        {
            GetReservableMaterialsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>.Success(
            [
                new ReservableInventoryMaterial(42, "Piolet técnico", true, 3),
                new ReservableInventoryMaterial(43, "Casco inactivo", false, 0)
            ]))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationCreateView>();
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Piolet técnico", cut.Markup);
            Assert.DoesNotContain("Casco inactivo", cut.Markup);
            Assert.Null(cut.Find("button[type='submit']").GetAttribute("disabled"));
        });

        cut.Find("#member-reservation-material").Change("42");
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("member_material_reservations_available_units", cut.Markup);
            Assert.Contains("3", cut.Markup);
        });
        cut.Find("#member-reservation-quantity").Change("2");
        cut.Find("#member-reservation-pickup-date").Change("2026-11-12");
        cut.Find("#member-reservation-return-date").Change("2026-11-11");
        cut.Find(".member-material-reservations__form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Null(service.LastCreateRequest);
            Assert.Contains("no puede ser anterior", cut.Markup, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public void CreateForm_SendsOnlyMemberRequestAndReloadsReservationsAfterCreated()
    {
        var reservation = CreateReservation();
        var service = new RecordingMemberMaterialReservationService
        {
            GetReservableMaterialsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>.Success(
                [new ReservableInventoryMaterial(42, "Piolet técnico", true, 3)])),
            CreateReservationHandler = (_, _) => Task.FromResult(ServiceResult<MaterialReservation>.Success(reservation)),
            GetMyReservationsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<MaterialReservation>>.Success([reservation]))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationCreateView>();
        cut.WaitForAssertion(() => Assert.Contains("Piolet técnico", cut.Markup));
        cut.Find("#member-reservation-material").Change("42");
        cut.Find("#member-reservation-quantity").Change("2");
        cut.Find("#member-reservation-pickup-date").Change("2026-11-12");
        cut.Find("#member-reservation-return-date").Change("2026-11-15");
        cut.Find("#member-reservation-notes").Change("  Recogeré el material por la tarde.  ");
        cut.Find(".member-material-reservations__form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(new CreateMemberMaterialReservationRequest(
                42,
                2,
                new DateOnly(2026, 11, 12),
                new DateOnly(2026, 11, 15),
                "Recogeré el material por la tarde."), service.LastCreateRequest);
            Assert.Equal(1, service.GetMyReservationsCallCount);
            Assert.Equal("http://localhost/mi-club/reservas-material", Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().Uri);
        });
    }

    [Fact]
    public void CreateForm_PreservesValuesAndFieldErrorAfterServerConflict()
    {
        var service = new RecordingMemberMaterialReservationService
        {
            GetReservableMaterialsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<ReservableInventoryMaterial>>.Success(
                [new ReservableInventoryMaterial(42, "Piolet técnico", true, 3)])),
            CreateReservationHandler = (_, _) => Task.FromResult(ServiceResult<MaterialReservation>.Failure(
                new ServiceError(
                    "reservations.conflict",
                    "El material ya no está disponible.",
                    [new ServiceValidationError("cantidadSolicitada", "No hay unidades suficientes.")])))
        };
        RegisterServices(service);

        var cut = Render<MemberMaterialReservationCreateView>();
        cut.WaitForAssertion(() => Assert.Contains("Piolet técnico", cut.Markup));
        cut.Find("#member-reservation-material").Change("42");
        cut.Find("#member-reservation-quantity").Change("2");
        cut.Find("#member-reservation-pickup-date").Change("2026-11-12");
        cut.Find("#member-reservation-return-date").Change("2026-11-15");
        cut.Find(".member-material-reservations__form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("No hay unidades suficientes.", cut.Markup);
            Assert.Equal("2", cut.Find("#member-reservation-quantity").GetAttribute("value"));
            Assert.Equal(2, service.GetReservableMaterialsCallCount);
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

    private static MaterialReservation CreateReservation()
        => new(
            Guid.Parse("ae503b49-fbc4-4aa8-aa55-c1eb3a2b1b91"),
            Guid.Parse("2f7a1bbd-556c-4dea-9d3e-a7de38e6f45a"),
            42,
            "Piolet técnico",
            2,
            new DateTime(2026, 10, 1, 9, 30, 0, DateTimeKind.Utc),
            new DateOnly(2026, 11, 12),
            null,
            new DateOnly(2026, 11, 15),
            null,
            "Recogeré el material por la tarde.",
            null,
            MaterialReservationStatus.Requested,
            false,
            false,
            null,
            null);
}
