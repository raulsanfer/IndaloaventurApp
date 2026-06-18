namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public partial class MyAccountPage
{
    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (!SessionService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/", true);
        }
    }

    protected async Task SignOutAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("indaloGoogleAuth.reset");
        }
        catch (JSException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        await SessionService.SignOutAsync();
        NavigationManager.NavigateTo("/", true);
    }
}
