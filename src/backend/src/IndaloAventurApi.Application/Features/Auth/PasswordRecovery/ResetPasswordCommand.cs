using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Auth.PasswordRecovery;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword) : ICommand<PasswordRecoveryResponse>;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty();
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("La confirmacion de la nueva contrasena no coincide.");
    }
}
