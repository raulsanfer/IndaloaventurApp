using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Users.ListManagedUsers;

public sealed class ListManagedUsersQueryHandler(IIdentityService identityService)
    : IRequestHandler<ListManagedUsersQuery, IReadOnlyCollection<ManagedUserDto>>
{
    public Task<IReadOnlyCollection<ManagedUserDto>> Handle(ListManagedUsersQuery request, CancellationToken cancellationToken)
    {
        var normalizedEmail = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim();

        return identityService.ListUsersAsync(normalizedEmail, cancellationToken);
    }
}
