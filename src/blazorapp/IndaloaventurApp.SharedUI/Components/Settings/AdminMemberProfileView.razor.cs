namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Components;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
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

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected MemberSelfProfileFormModel? Form { get; private set; }

    protected EditContext? EditContext { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected bool IsStateChanging { get; private set; }

    protected bool IsUpdatingInventoryManager { get; private set; }

    protected bool IsInventoryManagerSelected { get; private set; }

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
        if (!await RefreshManagedUserAsync())
        {
            ManagedUser = null;
        }

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

    protected void HandleInventoryManagerSelectionChanged(ChangeEventArgs args)
    {
        if (ManagedUser?.IsMember != true || IsUpdatingInventoryManager)
        {
            return;
        }

        IsInventoryManagerSelected = args.Value is bool selected && selected;
    }

    protected async Task HandleSaveInventoryManagerAsync()
    {
        if (ManagedUser?.IsMember != true || IsUpdatingInventoryManager)
        {
            return;
        }

        ClearMessages();
        IsUpdatingInventoryManager = true;

        var updatedRoles = BuildUpdatedRoles(ManagedUser);
        var rolesChanged = !HaveEquivalentRoles(ManagedUser.Roles, updatedRoles);
        var result = await AdminUserManagementService.UpdateUserAsync(
            ManagedUser.UserId,
            new UpdateManagedUserRequest(ManagedUser.Email, ManagedUser.IsMember, updatedRoles));

        IsUpdatingInventoryManager = false;
        if (!result.IsSuccess)
        {
            RestoreInventoryManagerSelection();
            await HandleInventoryManagerUpdateFailureAsync(result.Error);
            return;
        }

        if (rolesChanged && SessionService.CurrentSession?.UserId == ManagedUser.UserId)
        {
            await SessionService.SignOutAsync();
            NavigationManager.NavigateTo("/", true);
            return;
        }

        if (!await RefreshManagedUserAsync())
        {
            ErrorMessageKey = "settings_admin_member_file_load_error";
            return;
        }

        StatusMessageKey = "settings_users_inventory_manager_save_success";
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
        IsInventoryManagerSelected = HasInventoryManagerRole(ManagedUser);
        await ApplyProfileAsync(result.Value);
    }

    private async Task<bool> RefreshManagedUserAsync()
    {
        var userResult = await AdminUserManagementService.GetUserAsync(UserId);
        if (!userResult.IsSuccess || userResult.Value is null)
        {
            return false;
        }

        ManagedUser = userResult.Value;
        IsInventoryManagerSelected = HasInventoryManagerRole(ManagedUser);
        return true;
    }

    private IReadOnlyList<string> BuildUpdatedRoles(ManagedUserItem user)
    {
        var roles = user.Roles
            .Where(role => !string.Equals(role, "InventoryManager", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (IsInventoryManagerSelected)
        {
            roles.Add("InventoryManager");
        }

        return roles;
    }

    private async Task HandleInventoryManagerUpdateFailureAsync(ServiceError? error)
    {
        if (error?.Code == "auth.session_invalid")
        {
            await SessionService.SignOutAsync();
            NavigationManager.NavigateTo("/", true);
            return;
        }

        ErrorMessageKey = error?.Code switch
        {
            "users.forbidden" => "settings_users_inventory_manager_forbidden",
            "users.validation" => "settings_users_inventory_manager_validation_error",
            "users.not_found" => "settings_users_inventory_manager_not_found",
            "users.conflict" => "settings_users_inventory_manager_conflict",
            _ => "settings_users_inventory_manager_save_error"
        };
    }

    private void RestoreInventoryManagerSelection()
    {
        IsInventoryManagerSelected = ManagedUser is not null && HasInventoryManagerRole(ManagedUser);
    }

    private static bool HasInventoryManagerRole(ManagedUserItem user)
        => user.Roles.Any(role => string.Equals(role, "InventoryManager", StringComparison.OrdinalIgnoreCase));

    private static bool HaveEquivalentRoles(IReadOnlyList<string> first, IReadOnlyList<string> second)
        => first.Count == second.Count
            && first.OrderBy(static role => role, StringComparer.OrdinalIgnoreCase)
                .SequenceEqual(second.OrderBy(static role => role, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

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
