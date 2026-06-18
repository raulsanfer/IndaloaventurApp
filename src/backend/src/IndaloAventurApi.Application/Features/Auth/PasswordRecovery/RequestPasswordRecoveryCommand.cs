using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Auth.PasswordRecovery;

public sealed record RequestPasswordRecoveryCommand(string Email) : ICommand<PasswordRecoveryResponse>;

public sealed class RequestPasswordRecoveryCommandValidator : AbstractValidator<RequestPasswordRecoveryCommand>
{
    public RequestPasswordRecoveryCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
    }
}
