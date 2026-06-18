namespace IndaloaventurApp.SharedUI.Components.Login;

using IndaloaventurApp.SharedUI.Abstractions.Auth;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class PasswordRecoveryRequestView
{
    [Inject]
    private IAuthService AuthService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected PasswordRecoveryRequestFormModel Form { get; } = new();

    protected bool IsSubmitting { get; private set; }

    protected string? SuccessMessage { get; private set; }

    protected string? ErrorMessage { get; private set; }

    protected async Task SubmitAsync()
    {
        SuccessMessage = null;
        ErrorMessage = null;
        IsSubmitting = true;

        var result = await AuthService.RequestPasswordRecoveryAsync(new PasswordRecoveryRequest(Form.Email));

        IsSubmitting = false;

        if (result.IsSuccess)
        {
            SuccessMessage = string.IsNullOrWhiteSpace(result.Value)
                ? L["auth_recovery_neutral_message"].Value
                : result.Value;
            return;
        }

        ErrorMessage = result.Error?.Message ?? L["auth_recovery_error"].Value;
    }
}
