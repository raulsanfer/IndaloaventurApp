namespace IndaloaventurApp.SharedUI.Components.Login;

using IndaloaventurApp.SharedUI.Abstractions.Auth;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

public partial class LoginView : IDisposable
{
    private const string GoogleProvider = "google";
    private const string InvalidSocialCredentialsError = "auth.social_invalid";

    private DotNetObjectReference<LoginView>? dotNetReference;
    private bool hasAttemptedGoogleInitialization;
    private bool googleButtonUnavailable;

    [Inject]
    private IAuthService AuthService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Inject]
    private GoogleAuthOptions GoogleAuthOptions { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public EventCallback OnLoginSucceeded { get; set; }

    [Parameter]
    public string? StatusMessageKey { get; set; }

    protected LoginFormModel Form { get; } = new();

    protected bool IsSubmitting => IsPasswordSubmitting || IsSocialSubmitting;

    protected bool IsPasswordSubmitting { get; private set; }

    protected bool IsSocialSubmitting { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string GoogleButtonElementId { get; } = $"google-login-{Guid.NewGuid():N}";

    protected bool CanRenderGoogleButton => HasGoogleClientId && !googleButtonUnavailable;

    private bool HasGoogleClientId => !string.IsNullOrWhiteSpace(GoogleAuthOptions.ClientId);

    protected async Task SubmitAsync()
    {
        ErrorMessageKey = null;
        IsPasswordSubmitting = true;

        var result = await AuthService.LoginAsync(new LoginRequest(Form.EmailOrUserName, Form.Password));

        IsPasswordSubmitting = false;

        if (result.IsSuccess)
        {
            await CompleteLoginAsync(result.Value);
            return;
        }

        ErrorMessageKey = result.Error?.Code == "auth.invalid_credentials"
            ? "login_error_invalid_credentials"
            : "login_error_unavailable";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || !HasGoogleClientId || googleButtonUnavailable || hasAttemptedGoogleInitialization)
        {
            return;
        }

        try
        {
            hasAttemptedGoogleInitialization = true;
            dotNetReference ??= DotNetObjectReference.Create(this);

            var initialized = await JSRuntime.InvokeAsync<bool>(
                "indaloGoogleAuth.initializeButton",
                GoogleButtonElementId,
                GoogleAuthOptions.ClientId,
                dotNetReference);

            if (!initialized)
            {
                googleButtonUnavailable = true;
                ErrorMessageKey ??= "login_social_google_unavailable_error";
                StateHasChanged();
            }
        }
        catch (InvalidOperationException)
        {
            hasAttemptedGoogleInitialization = false;
        }
        catch (JSException)
        {
            googleButtonUnavailable = true;
            ErrorMessageKey ??= "login_social_google_unavailable_error";
            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task HandleGoogleCredentialAsync(string credential)
    {
        ErrorMessageKey = null;
        IsSocialSubmitting = true;
        StateHasChanged();

        var result = await AuthService.LoginSocialAsync(new SocialLoginRequest(GoogleProvider, credential));

        IsSocialSubmitting = false;

        if (result.IsSuccess)
        {
            await CompleteLoginAsync(result.Value);
            return;
        }

        ErrorMessageKey = result.Error?.Code == InvalidSocialCredentialsError
            ? "login_social_google_failed_error"
            : "login_social_google_unavailable_error";

        StateHasChanged();
    }

    [JSInvokable]
    public Task HandleGoogleSignInErrorAsync(string errorCode)
    {
        ErrorMessageKey = errorCode switch
        {
            "missing_client_id" => "login_social_google_unavailable_error",
            "google_not_loaded" => "login_social_google_unavailable_error",
            _ => "login_social_google_failed_error"
        };

        IsSocialSubmitting = false;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        dotNetReference?.Dispose();
    }

    private async Task CompleteLoginAsync(AuthSession? session)
    {
        if (session is not null)
        {
            await SessionService.SetSessionAsync(session, Form.RememberMe);
        }

        await OnLoginSucceeded.InvokeAsync();
    }
}
