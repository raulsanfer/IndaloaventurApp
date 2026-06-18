using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Users.UpdateManagedUser;

public sealed class UpdateManagedUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<UpdateManagedUserCommand, bool>
{
    public async Task<bool> Handle(UpdateManagedUserCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.UpdateUserAsync(request.UserId, request.Email, request.IsMember, request.Roles, cancellationToken);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors));
        }

        return true;
    }
}
