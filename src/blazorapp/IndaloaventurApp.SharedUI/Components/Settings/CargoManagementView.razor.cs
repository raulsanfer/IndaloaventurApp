namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class CargoManagementView
{
    [Inject]
    private ICargoAdminService CargoAdminService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<CargoItem> Cargos { get; private set; } = Array.Empty<CargoItem>();

    protected CargoEditorModel Editor { get; private set; } = new();

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string? LoadErrorKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    protected int? EditingCargoId { get; private set; }

    protected bool IsEditing => EditingCargoId.HasValue;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    protected async Task ReloadAsync()
    {
        await LoadAsync();
    }

    protected void BeginCreate()
    {
        EditingCargoId = null;
        Editor = new CargoEditorModel();
        ClearMessages();
    }

    protected void BeginEdit(CargoItem cargo)
    {
        EditingCargoId = cargo.Id;
        Editor = new CargoEditorModel
        {
            Description = cargo.Description
        };

        ClearMessages();
    }

    protected void CancelEdit()
    {
        BeginCreate();
    }

    protected async Task HandleSubmitAsync(EditContext _)
    {
        ClearMessages();

        var description = Editor.Description?.Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            ErrorMessageKey = "settings_cargos_description_required";
            return;
        }

        var wasEditing = IsEditing;

        IsSubmitting = true;

        if (wasEditing)
        {
            var updateResult = await CargoAdminService.UpdateCargoAsync(new UpdateCargoRequest(EditingCargoId!.Value, description));
            IsSubmitting = false;

            if (!updateResult.IsSuccess)
            {
                ErrorMessageKey = "settings_cargos_update_error";
                return;
            }
        }
        else
        {
            var createResult = await CargoAdminService.CreateCargoAsync(new CreateCargoRequest(description));
            IsSubmitting = false;

            if (!createResult.IsSuccess)
            {
                ErrorMessageKey = "settings_cargos_create_error";
                return;
            }
        }

        await LoadAsync();
        BeginCreate();
        StatusMessageKey = wasEditing ? "settings_cargos_update_success" : "settings_cargos_create_success";
    }

    private async Task LoadAsync()
    {
        ClearMessages();
        IsLoading = true;

        var result = await CargoAdminService.GetCargosAsync();

        IsLoading = false;
        if (!result.IsSuccess)
        {
            LoadErrorKey = "settings_cargos_load_error";
            ErrorMessageKey = LoadErrorKey;
            Cargos = Array.Empty<CargoItem>();
            return;
        }

        Cargos = result.Value ?? Array.Empty<CargoItem>();
    }

    private void ClearMessages()
    {
        ErrorMessageKey = null;
        LoadErrorKey = null;
        StatusMessageKey = null;
    }

    protected sealed class CargoEditorModel
    {
        public string? Description { get; set; }
    }
}
