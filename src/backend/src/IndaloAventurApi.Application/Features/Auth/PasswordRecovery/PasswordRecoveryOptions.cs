namespace IndaloAventurApi.Application.Features.Auth.PasswordRecovery;

public sealed class PasswordRecoveryOptions
{
    public const string SectionName = "PasswordRecovery";

    public string FrontendResetPasswordUrl { get; init; } = string.Empty;
}
