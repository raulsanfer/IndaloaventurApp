using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Users.ChangeManagedUserPassword;

public sealed class ChangeManagedUserPasswordCommandHandler(IIdentityService identityService)
    : IRequestHandler<ChangeManagedUserPasswordCommand, PasswordChangeOutcome>
{
    public async Task<PasswordChangeOutcome> Handle(ChangeManagedUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.ResetPasswordByAdministratorAsync(request.UserId, request.NewPassword!, cancellationToken);
        return result switch
        {
            PasswordChangeResult.Succeeded => PasswordChangeOutcome.Succeeded,
            PasswordChangeResult.UserNotFound => PasswordChangeOutcome.UserNotFound,
            _ => PasswordChangeOutcome.Rejected
        };
    }
}
