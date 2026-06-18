using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Users.CreateManagedUser;

public sealed record CreateManagedUserCommand(string Email, string Password, IReadOnlyCollection<string> Roles) : ICommand<Guid>;
