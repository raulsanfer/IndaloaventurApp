namespace IndaloaventurApp.SharedUI.Components.Login;

using IndaloaventurApp.SharedUI.Abstractions.Auth;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class ResetPasswordView
{
    [Inject]
    private IAuthService AuthService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public string? Email { get; set; }

    [Parameter]
    public string? Token { get; set; }

    protected ResetPasswordFormModel Form { get; } = new();

    protected bool IsSubmitting { get; private set; }

    protected bool HasInvalidLink => string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token);

    protected bool ShowRestartLink { get; private set; }

    protected string? ErrorMessage { get; private set; }

    protected async Task SubmitAsync()
    {
        if (HasInvalidLink)
        {
            ShowRestartLink = true;
            ErrorMessage = null;
            return;
        }

        ErrorMessage = null;
        ShowRestartLink = false;
        IsSubmitting = true;

        var result = await AuthService.ResetPasswordAsync(new ResetPasswordRequest(
            Email!,
            Token!,
            Form.NewPassword,
            Form.ConfirmPassword));

        IsSubmitting = false;

        if (result.IsSuccess)
        {
            NavigationManager.NavigateTo("/?passwordReset=success");
            return;
        }

        ErrorMessage = result.Error?.Message ?? L["auth_reset_error"].Value;
        ShowRestartLink = true;
    }
}
