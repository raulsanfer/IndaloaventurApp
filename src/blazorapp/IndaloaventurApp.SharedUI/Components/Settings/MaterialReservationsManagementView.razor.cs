namespace IndaloaventurApp.SharedUI.Components.Settings;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class MaterialReservationsManagementView
{
    [Inject]
    private IInventoryReservationAdminService ReservationService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<MaterialReservation> Reservations { get; private set; } = Array.Empty<MaterialReservation>();

    protected MaterialReservation? SelectedReservation { get; private set; }

    protected Guid? SelectedReservationId { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected bool IsDetailLoading { get; private set; }

    protected bool IsSubmitting { get; private set; }

    protected bool IsForbidden { get; private set; }

    protected string? ErrorMessage { get; private set; }

    protected string? StatusMessage { get; private set; }

    protected ReservationAction PendingAction { get; private set; }

    protected ReservationActionEditorModel ActionEditor { get; private set; } = new();

    protected Dictionary<string, List<string>> ActionErrors { get; } = new(StringComparer.Ordinal);

    protected bool CanRegisterPickup => !IsSubmitting && SelectedReservation?.Status == MaterialReservationStatus.Requested;

    protected bool CanConfirmReturn => !IsSubmitting && SelectedReservation?.Status == MaterialReservationStatus.PickedUp;

    protected bool CanCancelReservation => !IsSubmitting && SelectedReservation?.Status is MaterialReservationStatus.Requested or MaterialReservationStatus.PickedUp;

    protected override Task OnInitializedAsync() => LoadReservationsAsync();

    protected async Task ReloadAsync()
    {
        if (IsForbidden)
        {
            return;
        }

        await LoadReservationsAsync();
    }

    protected async Task SelectReservationAsync(Guid reservationId)
    {
        if (IsForbidden || IsSubmitting)
        {
            return;
        }

        PendingAction = ReservationAction.None;
        ActionErrors.Clear();
        await LoadReservationDetailAsync(reservationId);
    }

    protected void BeginPickup() => BeginAction(ReservationAction.Pickup);

    protected void BeginCancellation() => BeginAction(ReservationAction.Cancellation);

    protected void BeginReturnConfirmation() => BeginAction(ReservationAction.ReturnConfirmation);

    protected void DismissAction()
    {
        if (IsSubmitting)
        {
            return;
        }

        PendingAction = ReservationAction.None;
        ActionEditor = new ReservationActionEditorModel();
        ActionErrors.Clear();
    }

    protected async Task SubmitPendingActionAsync(EditContext _)
    {
        if (SelectedReservation is null || PendingAction == ReservationAction.None || IsForbidden || !ValidatePendingAction())
        {
            return;
        }

        ClearFeedback();
        IsSubmitting = true;
        var reservationId = SelectedReservation.Id;
        var completedAction = PendingAction;
        var result = completedAction switch
        {
            ReservationAction.Pickup => await ReservationService.RegisterPickupAsync(
                reservationId,
                new RegisterAdminMaterialReservationPickupRequest(ActionEditor.PickupDate!.Value)),
            ReservationAction.Cancellation => await ReservationService.CancelReservationAsync(
                reservationId,
                new CancelAdminMaterialReservationRequest(NormalizeOptional(ActionEditor.AdministrationNote))),
            ReservationAction.ReturnConfirmation => await ReservationService.ConfirmReturnAsync(
                reservationId,
                new ConfirmAdminMaterialReservationReturnRequest(ActionEditor.ActualReturnDate!.Value)),
            _ => ServiceResult<bool>.Failure(new ServiceError("reservations.unavailable", L["material_reservations_operation_error"].Value))
        };

        IsSubmitting = false;
        if (!result.IsSuccess)
        {
            await HandleActionFailureAsync(result.Error, reservationId);
            return;
        }

        var listReloaded = await LoadReservationsAsync(preserveFeedback: true);
        var detailReloaded = await LoadReservationDetailAsync(reservationId, preserveFeedback: true);
        if (listReloaded && detailReloaded)
        {
            PendingAction = ReservationAction.None;
            ActionEditor = new ReservationActionEditorModel();
            ActionErrors.Clear();
            StatusMessage = L["material_reservations_action_updated"].Value;
        }
    }

    protected IEnumerable<string> GetFieldErrors(string propertyName)
        => ActionErrors.TryGetValue(propertyName, out var messages) ? messages : Array.Empty<string>();

    protected string GetStatusLabel(MaterialReservationStatus status)
        => L[status switch
        {
            MaterialReservationStatus.Requested => "material_reservations_status_requested",
            MaterialReservationStatus.PickedUp => "material_reservations_status_picked_up",
            MaterialReservationStatus.Returned => "material_reservations_status_returned",
            MaterialReservationStatus.Cancelled => "material_reservations_status_cancelled",
            _ => "material_reservations_status_unknown"
        }].Value;

    protected string GetStatusBadgeClass(MaterialReservationStatus status)
        => status switch
        {
            MaterialReservationStatus.Requested => "badge-warning",
            MaterialReservationStatus.PickedUp => "badge-info",
            MaterialReservationStatus.Returned => "badge-success",
            MaterialReservationStatus.Cancelled => "badge-error",
            _ => "badge-ghost"
        };

    protected string FormatDate(DateOnly? date) => date?.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture) ?? L["material_reservations_not_available"].Value;

    protected string FormatRequestDate(DateTime requestedAtUtc)
    {
        var displayDate = requestedAtUtc.Kind == DateTimeKind.Utc ? requestedAtUtc.ToLocalTime() : requestedAtUtc;
        return displayDate.ToString("g", CultureInfo.CurrentCulture);
    }

    protected string FormatConfirmation(bool confirmed)
        => L[confirmed ? "material_reservations_confirmed" : "material_reservations_pending"].Value;

    protected string GetActionTitle()
        => L[PendingAction switch
        {
            ReservationAction.Pickup => "material_reservations_register_pickup",
            ReservationAction.Cancellation => "material_reservations_cancel",
            ReservationAction.ReturnConfirmation => "material_reservations_confirm_return",
            _ => "material_reservations_actions"
        }].Value;

    protected string GetActionDescription()
        => L[PendingAction switch
        {
            ReservationAction.Pickup => "material_reservations_pickup_confirmation",
            ReservationAction.Cancellation => "material_reservations_cancellation_confirmation",
            ReservationAction.ReturnConfirmation => "material_reservations_return_confirmation",
            _ => "material_reservations_actions"
        }].Value;

    protected string GetActionSubmitLabel()
        => L[PendingAction switch
        {
            ReservationAction.Pickup => "material_reservations_confirm_pickup",
            ReservationAction.Cancellation => "material_reservations_confirm_cancellation",
            ReservationAction.ReturnConfirmation => "material_reservations_confirm_return",
            _ => "material_reservations_actions"
        }].Value;

    private async Task<bool> LoadReservationsAsync(bool preserveFeedback = false)
    {
        IsLoading = true;
        if (!preserveFeedback)
        {
            ClearFeedback();
        }

        var result = await ReservationService.GetReservationsAsync();
        IsLoading = false;
        if (result.IsSuccess)
        {
            Reservations = result.Value ?? Array.Empty<MaterialReservation>();
            return true;
        }

        await HandleLoadFailureAsync(result.Error, clearSelectionOnNotFound: false);
        return false;
    }

    private async Task<bool> LoadReservationDetailAsync(Guid reservationId, bool preserveFeedback = false)
    {
        IsDetailLoading = true;
        SelectedReservationId = reservationId;
        SelectedReservation = null;
        if (!preserveFeedback)
        {
            ClearFeedback();
        }

        var result = await ReservationService.GetReservationAsync(reservationId);
        IsDetailLoading = false;
        if (result.IsSuccess)
        {
            SelectedReservation = result.Value;
            return true;
        }

        await HandleLoadFailureAsync(result.Error, clearSelectionOnNotFound: true);
        return false;
    }

    private async Task HandleActionFailureAsync(ServiceError? error, Guid reservationId)
    {
        if (await HandleAuthenticationOrAuthorizationFailureAsync(error))
        {
            return;
        }

        if (error?.Code == "reservations.not_found")
        {
            await ClearSelectionAfterNotFoundAsync();
            return;
        }

        ApplyValidationErrors(error);
        ErrorMessage = error?.Message ?? L["material_reservations_operation_error"].Value;
        if (error?.Code == "reservations.conflict")
        {
            await LoadReservationsAsync(preserveFeedback: true);
            await LoadReservationDetailAsync(reservationId, preserveFeedback: true);
        }
    }

    private async Task HandleLoadFailureAsync(ServiceError? error, bool clearSelectionOnNotFound)
    {
        if (await HandleAuthenticationOrAuthorizationFailureAsync(error))
        {
            return;
        }

        if (clearSelectionOnNotFound && error?.Code == "reservations.not_found")
        {
            await ClearSelectionAfterNotFoundAsync();
            return;
        }

        ErrorMessage = error?.Message ?? L["material_reservations_load_error"].Value;
    }

    private async Task<bool> HandleAuthenticationOrAuthorizationFailureAsync(ServiceError? error)
    {
        if (error?.Code == "auth.session_invalid")
        {
            ClearSelectedReservation();
            await SessionService.SignOutAsync();
            NavigationManager.NavigateTo("/", true);
            return true;
        }

        if (error?.Code == "reservations.forbidden")
        {
            ClearSelectedReservation();
            IsForbidden = true;
            ErrorMessage = L["material_reservations_forbidden"].Value;
            return true;
        }

        return false;
    }

    private async Task ClearSelectionAfterNotFoundAsync()
    {
        ClearSelectedReservation();
        PendingAction = ReservationAction.None;
        ActionErrors.Clear();
        ErrorMessage = L["material_reservations_not_found"].Value;
        await LoadReservationsAsync(preserveFeedback: true);
    }

    private void ClearSelectedReservation()
    {
        SelectedReservation = null;
        SelectedReservationId = null;
        IsDetailLoading = false;
    }

    private void BeginAction(ReservationAction action)
    {
        if (SelectedReservation is null || IsForbidden || IsSubmitting)
        {
            return;
        }

        var isAllowed = action switch
        {
            ReservationAction.Pickup => CanRegisterPickup,
            ReservationAction.Cancellation => CanCancelReservation,
            ReservationAction.ReturnConfirmation => CanConfirmReturn,
            _ => false
        };
        if (!isAllowed)
        {
            return;
        }

        PendingAction = action;
        ActionEditor = new ReservationActionEditorModel();
        ActionErrors.Clear();
        ClearFeedback();
    }

    private bool ValidatePendingAction()
    {
        ActionErrors.Clear();
        switch (PendingAction)
        {
            case ReservationAction.Pickup when !ActionEditor.PickupDate.HasValue:
                AddError(nameof(ActionEditor.PickupDate), L["material_reservations_required_field"].Value);
                break;
            case ReservationAction.ReturnConfirmation:
                if (!ActionEditor.ActualReturnDate.HasValue)
                {
                    AddError(nameof(ActionEditor.ActualReturnDate), L["material_reservations_required_field"].Value);
                }
                else if (SelectedReservation?.PickupDate is DateOnly pickupDate && ActionEditor.ActualReturnDate.Value < pickupDate)
                {
                    AddError(nameof(ActionEditor.ActualReturnDate), L["material_reservations_return_before_pickup"].Value);
                }

                break;
            case ReservationAction.Cancellation:
                ActionEditor.AdministrationNote = NormalizeOptional(ActionEditor.AdministrationNote);
                if (SelectedReservation?.Status == MaterialReservationStatus.PickedUp && string.IsNullOrWhiteSpace(ActionEditor.AdministrationNote))
                {
                    AddError(nameof(ActionEditor.AdministrationNote), L["material_reservations_required_field"].Value);
                }

                break;
        }

        return ActionErrors.Count == 0;
    }

    private void ApplyValidationErrors(ServiceError? error)
    {
        if (error?.ValidationErrors is null)
        {
            return;
        }

        foreach (var validationError in error.ValidationErrors)
        {
            AddError(MapValidationProperty(validationError.PropertyName), validationError.ErrorMessage);
        }
    }

    private void AddError(string propertyName, string message)
    {
        if (!ActionErrors.TryGetValue(propertyName, out var messages))
        {
            messages = new List<string>();
            ActionErrors[propertyName] = messages;
        }

        messages.Add(message);
    }

    private void ClearFeedback()
    {
        ErrorMessage = null;
        StatusMessage = null;
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string MapValidationProperty(string propertyName)
        => propertyName.ToLowerInvariant() switch
        {
            "fecharetirada" => nameof(ReservationActionEditorModel.PickupDate),
            "fechadevolucionefectiva" => nameof(ReservationActionEditorModel.ActualReturnDate),
            "observacionadministracion" => nameof(ReservationActionEditorModel.AdministrationNote),
            _ => nameof(ReservationActionEditorModel.AdministrationNote)
        };

    protected enum ReservationAction
    {
        None,
        Pickup,
        Cancellation,
        ReturnConfirmation
    }

    protected sealed class ReservationActionEditorModel
    {
        public DateOnly? PickupDate { get; set; }

        public DateOnly? ActualReturnDate { get; set; }

        public string? AdministrationNote { get; set; }
    }
}
