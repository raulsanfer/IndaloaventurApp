namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class AdminMemberProfilePage
{
    [Parameter]
    public Guid UserId { get; set; }

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (!SessionService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/", true);
            return;
        }

        if (SessionService.CurrentSession?.IsInRole("Admin") != true)
        {
            NavigationManager.NavigateTo("/configuracion", true);
        }
    }
}
