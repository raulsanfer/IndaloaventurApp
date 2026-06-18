using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Users.CreateManagedUser;

public sealed class CreateManagedUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<CreateManagedUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateManagedUserCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.CreateUserAsync(request.Email, request.Password, request.Roles, cancellationToken);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors));
        }

        return result.UserId;
    }
}
