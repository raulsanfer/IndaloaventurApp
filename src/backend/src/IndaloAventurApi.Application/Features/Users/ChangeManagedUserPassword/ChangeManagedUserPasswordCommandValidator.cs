using FluentValidation;

namespace IndaloAventurApi.Application.Features.Users.ChangeManagedUserPassword;

public sealed class ChangeManagedUserPasswordCommandValidator : AbstractValidator<ChangeManagedUserPasswordCommand>
{
    public ChangeManagedUserPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Length(1, 20)
            .Matches("^[A-Za-z0-9]+$");
    }
}
