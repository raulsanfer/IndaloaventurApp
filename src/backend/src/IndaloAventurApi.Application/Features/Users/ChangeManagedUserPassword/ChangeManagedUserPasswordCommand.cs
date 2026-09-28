using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Users.ChangeManagedUserPassword;

public sealed record ChangeManagedUserPasswordCommand(Guid UserId, string? NewPassword) : ICommand<PasswordChangeOutcome>;

public enum PasswordChangeOutcome
{
    Succeeded,
    UserNotFound,
    Rejected
}
