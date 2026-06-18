namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

public partial class AdminUsersManagementView
{
    [Inject]
    private IAdminUserManagementService AdminUserManagementService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected string SearchEmail { get; private set; } = string.Empty;

    protected IReadOnlyList<ManagedUserItem> Results { get; private set; } = Array.Empty<ManagedUserItem>();

    protected bool IsSearching { get; private set; }

    protected bool HasLoaded { get; private set; }

    protected bool IsFilterApplied { get; private set; }

    protected Guid? CreatingUserId { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    protected void HandleSearchEmailInput(ChangeEventArgs args)
    {
        SearchEmail = args.Value?.ToString() ?? string.Empty;
    }

    protected async Task HandleSearchSubmitAsync()
    {
        var requestedEmail = MemberSelfProfileMapper.NormalizeEmail(SearchEmail);
        await LoadUsersAsync(requestedEmail);
    }

    protected async Task HandleCreateMemberFileAsync(ManagedUserItem user)
    {
        ClearMessages();
        CreatingUserId = user.UserId;

        var result = await AdminUserManagementService.CreateMemberFileAsync(user.UserId, user.Email);

        CreatingUserId = null;
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessageKey = "settings_users_create_error";
            return;
        }

        NavigationManager.NavigateTo(BuildMemberFileHref(user.UserId));
    }

    protected string BuildMemberFileHref(Guid userId)
        => $"/configuracion/usuarios/{userId}/ficha";

    protected string BuildFederativeLicensesHref(Guid userId)
        => $"/configuracion/licencias-federativas?userId={userId:D}";

    protected string GetOperationalBadgeClass(ManagedUserItem user)
        => user.IsActive ? "badge-success" : "badge-error";

    protected string GetOperationalBadgeText(ManagedUserItem user)
        => user.IsActive
            ? L["settings_users_active_badge"]
            : L["settings_users_inactive_badge"];

    private async Task LoadUsersAsync(string? email = null)
    {
        ClearMessages();
        IsSearching = true;
        IsFilterApplied = !string.IsNullOrWhiteSpace(email);
        Results = Array.Empty<ManagedUserItem>();

        var result = await AdminUserManagementService.GetUsersAsync(email);

        IsSearching = false;
        HasLoaded = true;

        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessageKey = "settings_users_search_error";
            return;
        }

        Results = result.Value;
    }

    private void ClearMessages()
    {
        ErrorMessageKey = null;
        StatusMessageKey = null;
    }
}
