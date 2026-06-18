namespace IndaloaventurApp.SharedUI.Components.MyAccount;

using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class MyAccountView
{
    [Inject]
    private IMemberProfileService MemberProfileService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public EventCallback OnSignOut { get; set; }

    protected MemberProfile? Profile { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessage { get; private set; }

    protected bool ShowJoinMemberLink =>
        SessionService.CurrentSession?.IsMember == false &&
        SessionService.CurrentSession?.IsInRole("Member") == true;

    protected string Initials
    {
        get
        {
            if (Profile is null || string.IsNullOrWhiteSpace(Profile.FullName))
            {
                return "IA";
            }

            var parts = Profile.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1
                ? parts[0][..1].ToUpperInvariant()
                : string.Concat(parts[0][0], parts[^1][0]).ToUpperInvariant();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var result = await MemberProfileService.GetMyProfileAsync();
        IsLoading = false;

        if (result.IsSuccess)
        {
            Profile = result.Value;
            return;
        }

        ErrorMessage = L["mi_cuenta_load_error"];
    }

    protected async Task OnSignOutClicked()
    {
        await OnSignOut.InvokeAsync();
    }
}
