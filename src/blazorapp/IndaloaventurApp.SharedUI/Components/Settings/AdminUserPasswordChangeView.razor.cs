namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class AdminUserPasswordChangeView
{
    [Parameter]
    public Guid UserId { get; set; }

    [Inject]
    private IAdminUserManagementService AdminUserManagementService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected AdminUserPasswordChangeFormModel Form { get; } = new();

    protected ManagedUserItem? ManagedUser { get; private set; }

    protected string? TargetDisplayName { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadAsync();
    }

    protected async Task HandleValidSubmitAsync()
    {
        if (ManagedUser is null)
        {
            return;
        }

        ErrorMessageKey = null;
        StatusMessageKey = null;
        IsSubmitting = true;

        var result = await AdminUserManagementService.ChangeUserPasswordAsync(
            UserId,
            new AdminUserPasswordChangeRequest(Form.NewPassword));

        IsSubmitting = false;
        Form.NewPassword = string.Empty;

        if (!result.IsSuccess)
        {
            ErrorMessageKey = result.Error?.Code switch
            {
                "users.password_validation" => "settings_admin_password_validation_error",
                "users.not_found" => "settings_admin_password_not_found",
                "users.forbidden" or "auth.session_invalid" => "settings_admin_password_forbidden_error",
                _ => "settings_admin_password_submit_error"
            };
            return;
        }

        StatusMessageKey = "settings_admin_password_submit_success";
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessageKey = null;
        StatusMessageKey = null;

        var userTask = AdminUserManagementService.GetUserAsync(UserId);
        var profileTask = AdminUserManagementService.GetMemberFileAsync(UserId);
        await Task.WhenAll(userTask, profileTask);

        IsLoading = false;
        if (!userTask.Result.IsSuccess || userTask.Result.Value is null)
        {
            ErrorMessageKey = userTask.Result.Error?.Code == "users.not_found"
                ? "settings_admin_password_not_found"
                : "settings_admin_password_load_error";
            return;
        }

        ManagedUser = userTask.Result.Value;
        var profile = profileTask.Result.Value;
        TargetDisplayName = string.Join(' ', new[] { profile?.Nombre, profile?.Apellidos }
            .Where(value => !string.IsNullOrWhiteSpace(value)));

        if (string.IsNullOrWhiteSpace(TargetDisplayName))
        {
            TargetDisplayName = ManagedUser.Email;
        }
    }
}
