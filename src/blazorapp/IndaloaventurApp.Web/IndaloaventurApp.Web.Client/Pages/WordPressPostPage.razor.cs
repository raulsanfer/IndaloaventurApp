namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class WordPressPostPage
{
    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public string Slug { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        if (!SessionService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/", true);
        }
    }
}
