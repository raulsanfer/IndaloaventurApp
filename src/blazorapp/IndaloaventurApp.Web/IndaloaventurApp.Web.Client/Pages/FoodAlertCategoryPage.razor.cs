namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class FoodAlertCategoryPage
{
    [Parameter]
    public string CategoryCode { get; set; } = string.Empty;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (!SessionService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/", true);
        }
    }
}
