namespace IndaloaventurApp.Web.Client.Pages;

using Microsoft.AspNetCore.Components;

public partial class ResetPasswordPage
{
    [SupplyParameterFromQuery(Name = "email")]
    public string? Email { get; set; }

    [SupplyParameterFromQuery(Name = "token")]
    public string? Token { get; set; }
}
