namespace IndaloaventurApp.SharedUI.Components.Shell;

using Microsoft.AspNetCore.Components;

public partial class AuthenticatedShell
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string ActiveHref { get; set; } = "/home";
}
