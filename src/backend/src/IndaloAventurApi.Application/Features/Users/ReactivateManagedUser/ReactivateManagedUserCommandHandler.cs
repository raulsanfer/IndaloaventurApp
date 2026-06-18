using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Users.ReactivateManagedUser;

public sealed class ReactivateManagedUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<ReactivateManagedUserCommand, bool>
{
    public async Task<bool> Handle(ReactivateManagedUserCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.ReactivateUserAsync(request.UserId, cancellationToken);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors));
        }

        return true;
    }
}
