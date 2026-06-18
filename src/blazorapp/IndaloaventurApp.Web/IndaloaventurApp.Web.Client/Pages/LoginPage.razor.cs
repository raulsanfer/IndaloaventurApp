namespace IndaloaventurApp.Web.Client.Pages;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using Microsoft.AspNetCore.Components;

public partial class LoginPage
{
    [SupplyParameterFromQuery(Name = "passwordReset")]
    public string? PasswordReset { get; set; }

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected string? StatusMessageKey =>
        string.Equals(PasswordReset, "success", StringComparison.OrdinalIgnoreCase)
            ? "auth_reset_login_success"
            : null;

    protected override void OnInitialized()
    {
        if (SessionService.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/home");
        }
    }

    private void HandleLoginSucceeded()
    {
        NavigationManager.NavigateTo("/home");
    }
}
