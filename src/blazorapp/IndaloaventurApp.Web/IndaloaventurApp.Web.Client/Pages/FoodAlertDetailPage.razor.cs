namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class FoodAlertDetailPage
{
    [Parameter]
    public string AlertId { get; set; } = string.Empty;

    [SupplyParameterFromQuery(Name = "category")]
    public string? CategoryCode { get; set; }

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
