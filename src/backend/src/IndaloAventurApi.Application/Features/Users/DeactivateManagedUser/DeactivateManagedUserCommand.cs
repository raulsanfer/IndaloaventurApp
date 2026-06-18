using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Users.DeactivateManagedUser;

public sealed record DeactivateManagedUserCommand(Guid UserId) : ICommand<bool>;
