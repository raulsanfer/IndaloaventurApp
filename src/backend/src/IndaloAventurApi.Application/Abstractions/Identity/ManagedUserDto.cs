namespace IndaloAventurApi.Application.Abstractions.Identity;

public sealed record ManagedUserDto(Guid UserId, string Email, bool IsMember, IReadOnlyCollection<string> Roles);
