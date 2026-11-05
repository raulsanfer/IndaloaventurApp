namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class ClubMaterialReservationsPage
{
    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected bool CanAccessReservations { get; private set; }

    protected override void OnInitialized()
    {
        if (!SessionService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/", true);
            return;
        }

        if (SessionService.CurrentSession?.CanAccessOwnMaterialReservations() != true)
        {
            NavigationManager.NavigateTo("/mi-club", true);
            return;
        }

        CanAccessReservations = true;
    }
}
