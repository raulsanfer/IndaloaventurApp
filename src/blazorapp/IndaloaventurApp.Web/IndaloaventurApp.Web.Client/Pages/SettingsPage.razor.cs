namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class SettingsPage
{
    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected bool IsAdmin => SessionService.CurrentSession?.IsInRole("Admin") == true;

    protected override void OnInitialized()
    {
        if (!SessionService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/", true);
        }
    }
}
