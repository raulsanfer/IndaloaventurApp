using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Identity;

namespace IndaloAventurApi.Application.Features.Users.ListManagedUsers;

public sealed record ListManagedUsersQuery(string? Email = null) : IQuery<IReadOnlyCollection<ManagedUserDto>>;
