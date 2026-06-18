namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class SignalCategoriesManagementView
{
    [Inject]
    private ISignalService SignalService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<SignalCategoryItem> Categories { get; private set; } = Array.Empty<SignalCategoryItem>();

    protected SignalCategoryEditorModel Editor { get; private set; } = new();

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected int? DeletingCategoryId { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string? LoadErrorKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    protected int? EditingCategoryId { get; private set; }

    protected bool IsEditing => EditingCategoryId.HasValue;

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
        EditingCategoryId = null;
        Editor = new SignalCategoryEditorModel();
        ClearMessages();
    }

    protected void BeginEdit(SignalCategoryItem category)
    {
        EditingCategoryId = category.Id;
        Editor = new SignalCategoryEditorModel
        {
            Name = category.Name,
            IconName = category.IconName
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

        var name = Editor.Name?.Trim();
        var iconName = NormalizeIcon(Editor.IconName);

        if (string.IsNullOrWhiteSpace(name))
        {
            ErrorMessageKey = "settings_signal_categories_name_required";
            return;
        }

        var wasEditing = IsEditing;
        IsSubmitting = true;

        if (wasEditing)
        {
            var result = await SignalService.UpdateSignalCategoryAsync(new UpdateSignalCategoryRequest(EditingCategoryId!.Value, name, iconName));
            IsSubmitting = false;

            if (!result.IsSuccess)
            {
                ErrorMessageKey = MapCategoryErrorKey(result.Error?.Code, isDelete: false, isEditing: true);
                return;
            }
        }
        else
        {
            var result = await SignalService.CreateSignalCategoryAsync(new CreateSignalCategoryRequest(name, iconName));
            IsSubmitting = false;

            if (!result.IsSuccess)
            {
                ErrorMessageKey = MapCategoryErrorKey(result.Error?.Code, isDelete: false, isEditing: false);
                return;
            }
        }

        await LoadAsync();
        BeginCreate();
        StatusMessageKey = wasEditing ? "settings_signal_categories_update_success" : "settings_signal_categories_create_success";
    }

    protected async Task DeleteAsync(SignalCategoryItem category)
    {
        ClearMessages();
        DeletingCategoryId = category.Id;

        var result = await SignalService.DeleteSignalCategoryAsync(category.Id);

        DeletingCategoryId = null;
        if (!result.IsSuccess)
        {
            ErrorMessageKey = MapCategoryErrorKey(result.Error?.Code, isDelete: true, isEditing: false);
            return;
        }

        await LoadAsync();
        if (EditingCategoryId == category.Id)
        {
            BeginCreate();
        }

        StatusMessageKey = "settings_signal_categories_delete_success";
    }

    private async Task LoadAsync()
    {
        ClearMessages();
        IsLoading = true;

        var result = await SignalService.GetSignalCategoriesAsync();

        IsLoading = false;
        if (!result.IsSuccess)
        {
            LoadErrorKey = "settings_signal_categories_load_error";
            ErrorMessageKey = LoadErrorKey;
            Categories = Array.Empty<SignalCategoryItem>();
            return;
        }

        Categories = result.Value ?? Array.Empty<SignalCategoryItem>();
    }

    private void ClearMessages()
    {
        ErrorMessageKey = null;
        LoadErrorKey = null;
        StatusMessageKey = null;
    }

    private static string? NormalizeIcon(string? iconName)
    {
        if (string.IsNullOrWhiteSpace(iconName))
        {
            return null;
        }

        var normalized = iconName.Trim();
        if (normalized.StartsWith("bi ", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized["bi ".Length..].Trim();
        }

        return normalized.StartsWith("bi-", StringComparison.OrdinalIgnoreCase)
            ? normalized
            : $"bi-{normalized}";
    }

    private static string MapCategoryErrorKey(string? errorCode, bool isDelete, bool isEditing)
    {
        return errorCode switch
        {
            "signals.categories.validation" => "settings_signal_categories_validation_error",
            "signals.categories.delete_blocked" => "settings_signal_categories_delete_blocked",
            "signals.categories.not_found" => "settings_signal_categories_not_found",
            _ when isDelete => "settings_signal_categories_delete_error",
            _ when isEditing => "settings_signal_categories_update_error",
            _ => "settings_signal_categories_create_error"
        };
    }

    protected sealed class SignalCategoryEditorModel
    {
        public string? Name { get; set; }

        public string? IconName { get; set; }
    }
}
