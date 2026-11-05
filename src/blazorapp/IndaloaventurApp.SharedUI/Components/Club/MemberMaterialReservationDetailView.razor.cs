namespace IndaloaventurApp.SharedUI.Components.Club;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class MemberMaterialReservationDetailView
{
    [Parameter, EditorRequired]
    public Guid ReservationId { get; set; }

    [Inject]
    private IMemberMaterialReservationService ReservationService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected MaterialReservation? Reservation { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected bool IsForbidden { get; private set; }

    protected string? ErrorMessage { get; private set; }

    protected ReservationAction PendingAction { get; private set; }

    protected bool CanCancel => !IsSubmitting && Reservation?.Status == MaterialReservationStatus.Requested;

    protected bool CanConfirmReturn => !IsSubmitting && Reservation?.Status == MaterialReservationStatus.PickedUp;

    protected override Task OnInitializedAsync() => LoadReservationAsync();

    protected void BeginCancellation() => BeginAction(ReservationAction.Cancellation);

    protected void BeginReturnConfirmation() => BeginAction(ReservationAction.ReturnConfirmation);

    protected void DismissAction()
    {
        if (!IsSubmitting)
        {
            PendingAction = ReservationAction.None;
        }
    }

    protected async Task SubmitActionAsync()
    {
        if (Reservation is null || PendingAction == ReservationAction.None || IsSubmitting || IsForbidden)
        {
            return;
        }

        IsSubmitting = true;
        ErrorMessage = null;
        var result = PendingAction == ReservationAction.Cancellation
            ? await ReservationService.CancelReservationAsync(Reservation.Id)
            : await ReservationService.ConfirmReturnAsync(Reservation.Id);
        IsSubmitting = false;

        if (!result.IsSuccess)
        {
            await HandleActionFailureAsync(result.Error);
            return;
        }

        var listRefresh = await ReservationService.GetMyReservationsAsync();
        if (!listRefresh.IsSuccess)
        {
            await HandleActionFailureAsync(listRefresh.Error);
            return;
        }

        PendingAction = ReservationAction.None;
        await LoadReservationAsync();
    }

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

    protected string GetActionTitle()
        => L[PendingAction == ReservationAction.Cancellation
            ? "member_material_reservations_cancel"
            : "member_material_reservations_confirm_return"].Value;

    protected string GetActionDescription()
        => L[PendingAction == ReservationAction.Cancellation
            ? "member_material_reservations_cancel_confirmation"
            : "member_material_reservations_return_confirmation"].Value;

    protected string GetActionSubmitLabel()
        => L[PendingAction == ReservationAction.Cancellation
            ? "member_material_reservations_confirm_cancellation"
            : "member_material_reservations_confirm_return"].Value;

    private async Task LoadReservationAsync()
    {
        IsLoading = true;
        var result = await ReservationService.GetMyReservationAsync(ReservationId);
        IsLoading = false;
        if (result.IsSuccess)
        {
            Reservation = result.Value;
            return;
        }

        await HandleLoadFailureAsync(result.Error);
    }

    private async Task HandleActionFailureAsync(ServiceError? error)
    {
        if (await HandleAuthenticationOrAuthorizationFailureAsync(error))
        {
            return;
        }

        if (error?.Code == "reservations.not_found")
        {
            await ReturnToListAfterNotFoundAsync();
            return;
        }

        ErrorMessage = error?.Message ?? L["member_material_reservations_operation_error"].Value;
        if (error?.Code == "reservations.conflict")
        {
            // The state may have changed remotely, so the confirmation no longer represents a safe action.
            PendingAction = ReservationAction.None;
            var listRefresh = await ReservationService.GetMyReservationsAsync();
            if (!listRefresh.IsSuccess && await HandleAuthenticationOrAuthorizationFailureAsync(listRefresh.Error))
            {
                return;
            }

            await LoadReservationAsync();
        }
    }

    private async Task HandleLoadFailureAsync(ServiceError? error)
    {
        if (await HandleAuthenticationOrAuthorizationFailureAsync(error))
        {
            return;
        }

        if (error?.Code == "reservations.not_found")
        {
            await ReturnToListAfterNotFoundAsync();
            return;
        }

        Reservation = null;
        ErrorMessage = error?.Message ?? L["member_material_reservations_load_error"].Value;
    }

    private async Task<bool> HandleAuthenticationOrAuthorizationFailureAsync(ServiceError? error)
    {
        if (error?.Code == "auth.session_invalid")
        {
            Reservation = null;
            await SessionService.SignOutAsync();
            NavigationManager.NavigateTo("/", true);
            return true;
        }

        if (error?.Code == "reservations.forbidden")
        {
            Reservation = null;
            PendingAction = ReservationAction.None;
            IsForbidden = true;
            ErrorMessage = L["member_material_reservations_forbidden"].Value;
            return true;
        }

        return false;
    }

    private Task ReturnToListAfterNotFoundAsync()
    {
        Reservation = null;
        PendingAction = ReservationAction.None;
        NavigationManager.NavigateTo("/mi-club/reservas-material", true);
        return Task.CompletedTask;
    }

    private void BeginAction(ReservationAction action)
    {
        if (Reservation is null || IsForbidden || IsSubmitting ||
            (action == ReservationAction.Cancellation && !CanCancel) ||
            (action == ReservationAction.ReturnConfirmation && !CanConfirmReturn))
        {
            return;
        }

        ErrorMessage = null;
        PendingAction = action;
    }

    protected enum ReservationAction
    {
        None,
        Cancellation,
        ReturnConfirmation
    }
}
