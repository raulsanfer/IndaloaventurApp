using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Users.DeactivateManagedUser;

public sealed class DeactivateManagedUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<DeactivateManagedUserCommand, bool>
{
    public async Task<bool> Handle(DeactivateManagedUserCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.DeactivateUserAsync(request.UserId, cancellationToken);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors));
        }

        return true;
    }
}
