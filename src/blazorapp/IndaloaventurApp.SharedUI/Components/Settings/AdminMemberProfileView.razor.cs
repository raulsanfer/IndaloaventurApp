namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Components;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class AdminMemberProfileView
{
    [Parameter]
    public Guid UserId { get; set; }

    [Inject]
    private IAdminUserManagementService AdminUserManagementService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Inject]
    private ICargoAdminService CargoAdminService { get; set; } = default!;

    protected MemberSelfProfileFormModel? Form { get; private set; }

    protected EditContext? EditContext { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected bool IsStateChanging { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    protected ManagedUserItem? ManagedUser { get; private set; }

    protected MemberCargoFieldState CargoFieldState { get; private set; } =
        new(string.Empty, Array.Empty<IndaloaventurApp.SharedUI.Models.Cargos.CargoSelectionOption>(), false, false);

    protected override async Task OnParametersSetAsync()
    {
        await LoadAsync();
    }

    protected async Task HandleValidSubmitAsync()
    {
        if (Form is null)
        {
            return;
        }

        ClearMessages();
        IsSubmitting = true;

        var result = await AdminUserManagementService.UpdateMemberFileAsync(UserId, MemberSelfProfileMapper.ToRequest(Form));

        IsSubmitting = false;
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessageKey = result.Error?.Code switch
            {
                "profile.validation" => "settings_admin_member_file_save_validation_error",
                "profile.not_found" => "settings_admin_member_file_not_found",
                _ => "settings_admin_member_file_save_error"
            };
            return;
        }

        await ApplyProfileAsync(result.Value);
        StatusMessageKey = "settings_admin_member_file_save_success";
    }

    protected async Task HandleAccountStateToggleAsync()
    {
        if (ManagedUser is null)
        {
            return;
        }

        ClearMessages();
        IsStateChanging = true;

        var result = ManagedUser.IsActive
            ? await AdminUserManagementService.DeactivateUserAsync(UserId)
            : await AdminUserManagementService.ReactivateUserAsync(UserId);

        IsStateChanging = false;
        if (!result.IsSuccess)
        {
            ErrorMessageKey = "settings_admin_member_file_state_change_error";
            return;
        }

        ManagedUser = ManagedUser with { IsActive = !ManagedUser.IsActive };
        StatusMessageKey = ManagedUser.IsActive
            ? "settings_admin_member_file_reactivate_success"
            : "settings_admin_member_file_deactivate_success";
    }

    protected string GetManagedUserStateBadgeClass()
        => ManagedUser?.IsActive == true ? "badge-success" : "badge-error";

    protected string GetManagedUserStateText()
        => ManagedUser?.IsActive == true
            ? L["settings_users_active_badge"]
            : L["settings_users_inactive_badge"];

    private async Task LoadAsync()
    {
        ClearMessages();
        IsLoading = true;

        var profileTask = AdminUserManagementService.GetMemberFileAsync(UserId);
        var userTask = AdminUserManagementService.GetUserAsync(UserId);

        await Task.WhenAll(profileTask, userTask);

        var result = profileTask.Result;
        var userResult = userTask.Result;

        IsLoading = false;
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessageKey = result.Error?.Code switch
            {
                "profile.not_found" => "settings_admin_member_file_not_found",
                _ => "settings_admin_member_file_load_error"
            };
            return;
        }

        if (!userResult.IsSuccess || userResult.Value is null)
        {
            ErrorMessageKey = "settings_admin_member_file_load_error";
            return;
        }

        ManagedUser = userResult.Value;
        await ApplyProfileAsync(result.Value);
    }

    private async Task ApplyProfileAsync(MemberSelfProfile profile)
    {
        Form = MemberSelfProfileFormModel.FromProfile(profile);
        EditContext = new EditContext(Form);
        CargoFieldState = await MemberCargoFieldComponentBase.BuildAsync(CargoAdminService, profile, canEdit: true);
    }

    private void ClearMessages()
    {
        ErrorMessageKey = null;
        StatusMessageKey = null;
    }
}
