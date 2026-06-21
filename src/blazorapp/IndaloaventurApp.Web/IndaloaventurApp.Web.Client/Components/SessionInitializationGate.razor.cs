namespace IndaloaventurApp.Web.Client;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class SessionInitializationGate
{
    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await SessionService.EnsureInitializedAsync();
        await InvokeAsync(StateHasChanged);
    }
}
