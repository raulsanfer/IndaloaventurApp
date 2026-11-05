namespace IndaloaventurApp.SharedUI.Components.Club;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class MemberMaterialReservationsListView
{
    [Inject]
    private IMemberMaterialReservationService ReservationService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<MaterialReservation> Reservations { get; private set; } = Array.Empty<MaterialReservation>();

    protected bool IsLoading { get; private set; } = true;

    protected bool IsForbidden { get; private set; }

    protected string? ErrorMessage { get; private set; }

    protected override Task OnInitializedAsync() => LoadReservationsAsync();

    protected string DetailHref(Guid reservationId) => $"/mi-club/reservas-material/{reservationId}";

    protected string GetStatusLabel(MaterialReservationStatus status)
        => L[status switch
        {
            MaterialReservationStatus.Requested => "member_material_reservations_status_requested",
            MaterialReservationStatus.PickedUp => "member_material_reservations_status_picked_up",
            MaterialReservationStatus.Returned => "member_material_reservations_status_returned",
            MaterialReservationStatus.Cancelled => "member_material_reservations_status_cancelled",
            _ => "member_material_reservations_status_unknown"
        }].Value;

    protected static string GetStatusBadgeClass(MaterialReservationStatus status)
        => status switch
        {
            MaterialReservationStatus.Requested => "badge-warning",
            MaterialReservationStatus.PickedUp => "badge-info",
            MaterialReservationStatus.Returned => "badge-success",
            MaterialReservationStatus.Cancelled => "badge-error",
            _ => "badge-ghost"
        };

    protected string FormatDate(DateOnly date) => date.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);

    protected string FormatRequestDate(DateTime requestedAtUtc)
    {
        var displayDate = requestedAtUtc.Kind == DateTimeKind.Utc ? requestedAtUtc.ToLocalTime() : requestedAtUtc;
        return displayDate.ToString("g", CultureInfo.CurrentCulture);
    }

    private async Task LoadReservationsAsync()
    {
        var result = await ReservationService.GetMyReservationsAsync();
        IsLoading = false;
        if (result.IsSuccess)
        {
            Reservations = (result.Value ?? Array.Empty<MaterialReservation>())
                .OrderByDescending(static reservation => reservation.RequestedAtUtc)
                .ToArray();
            return;
        }

        await HandleFailureAsync(result.Error);
    }

    private async Task HandleFailureAsync(ServiceError? error)
    {
        if (error?.Code == "auth.session_invalid")
        {
            Reservations = Array.Empty<MaterialReservation>();
            await SessionService.SignOutAsync();
            NavigationManager.NavigateTo("/", true);
            return;
        }

        if (error?.Code == "reservations.forbidden")
        {
            Reservations = Array.Empty<MaterialReservation>();
            IsForbidden = true;
            ErrorMessage = L["member_material_reservations_forbidden"].Value;
            return;
        }

        ErrorMessage = error?.Message ?? L["member_material_reservations_load_error"].Value;
    }
}
