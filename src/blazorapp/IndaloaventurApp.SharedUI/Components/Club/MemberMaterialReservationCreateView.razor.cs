namespace IndaloaventurApp.SharedUI.Components.Club;

using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class MemberMaterialReservationCreateView
{
    [Inject]
    private IMemberMaterialReservationService ReservationService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected MemberMaterialReservationFormModel Form { get; } = new();

    protected EditContext EditContext { get; private set; } = default!;

    protected IReadOnlyList<ReservableInventoryMaterial> Materials { get; private set; } = Array.Empty<ReservableInventoryMaterial>();

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected bool IsForbidden { get; private set; }

    protected string? ErrorMessage { get; private set; }

    protected ReservableInventoryMaterial? SelectedMaterial => Form.InventoryMaterialId is int materialId
        ? Materials.FirstOrDefault(material => material.Id == materialId)
        : null;

    protected bool IsSubmitDisabled => IsSubmitting || Materials.Count == 0;

    private ValidationMessageStore ValidationMessages { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        EditContext = new EditContext(Form);
        ValidationMessages = new ValidationMessageStore(EditContext);
        await LoadMaterialsAsync();
    }

    protected async Task SubmitAsync()
    {
        if (IsSubmitting || IsForbidden)
        {
            return;
        }

        ValidationMessages.Clear();
        ErrorMessage = null;
        IsSubmitting = true;
        var result = await ReservationService.CreateReservationAsync(Form.ToRequest());
        IsSubmitting = false;
        if (!result.IsSuccess)
        {
            await HandleCreateFailureAsync(result.Error);
            return;
        }

        var refreshedReservations = await ReservationService.GetMyReservationsAsync();
        if (!refreshedReservations.IsSuccess)
        {
            await HandleFailureAsync(refreshedReservations.Error, clearForm: false);
            return;
        }

        NavigationManager.NavigateTo("/mi-club/reservas-material");
    }

    private async Task LoadMaterialsAsync(bool preserveFeedback = false)
    {
        IsLoading = true;
        if (!preserveFeedback)
        {
            ErrorMessage = null;
        }

        var result = await ReservationService.GetReservableMaterialsAsync();
        IsLoading = false;
        if (result.IsSuccess)
        {
            Materials = (result.Value ?? Array.Empty<ReservableInventoryMaterial>())
                .Where(static material => material.IsActive)
                .ToArray();
            return;
        }

        await HandleFailureAsync(result.Error, clearForm: false);
    }

    private async Task HandleCreateFailureAsync(ServiceError? error)
    {
        if (await HandleFailureAsync(error, clearForm: false))
        {
            return;
        }

        AddValidationErrors(error);
        if (error?.Code == "reservations.conflict")
        {
            await LoadMaterialsAsync(preserveFeedback: true);
        }
    }

    private async Task<bool> HandleFailureAsync(ServiceError? error, bool clearForm)
    {
        if (error?.Code == "auth.session_invalid")
        {
            if (clearForm)
            {
                Form.MemberNotes = null;
            }

            await SessionService.SignOutAsync();
            NavigationManager.NavigateTo("/", true);
            return true;
        }

        if (error?.Code == "reservations.forbidden")
        {
            IsForbidden = true;
            Materials = Array.Empty<ReservableInventoryMaterial>();
            ErrorMessage = L["member_material_reservations_forbidden"].Value;
            return true;
        }

        ErrorMessage = error?.Message ?? L["member_material_reservations_operation_error"].Value;
        return false;
    }

    private void AddValidationErrors(ServiceError? error)
    {
        if (error?.ValidationErrors is null)
        {
            return;
        }

        foreach (var errorItem in error.ValidationErrors)
        {
            ValidationMessages.Add(new FieldIdentifier(Form, MapValidationProperty(errorItem.PropertyName)), errorItem.ErrorMessage);
        }

        EditContext.NotifyValidationStateChanged();
    }

    private static string MapValidationProperty(string propertyName)
        => propertyName.ToLowerInvariant() switch
        {
            "inventariomaterialid" => nameof(MemberMaterialReservationFormModel.InventoryMaterialId),
            "cantidadsolicitada" => nameof(MemberMaterialReservationFormModel.RequestedQuantity),
            "fecharetiradaprevista" => nameof(MemberMaterialReservationFormModel.ScheduledPickupDate),
            "fechadevolucionestimada" => nameof(MemberMaterialReservationFormModel.EstimatedReturnDate),
            "observaciones" => nameof(MemberMaterialReservationFormModel.MemberNotes),
            _ => nameof(MemberMaterialReservationFormModel.MemberNotes)
        };
}
