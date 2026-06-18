using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Users.UpdateManagedUser;

public sealed record UpdateManagedUserCommand(Guid UserId, string Email, bool IsMember, IReadOnlyCollection<string> Roles) : ICommand<bool>;
