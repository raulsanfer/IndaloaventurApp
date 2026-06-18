using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Users.ReactivateManagedUser;

public sealed record ReactivateManagedUserCommand(Guid UserId) : ICommand<bool>;
