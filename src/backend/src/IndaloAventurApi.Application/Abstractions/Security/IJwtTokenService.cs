namespace IndaloAventurApi.Application.Abstractions.Security;

public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(Guid userId, string email, IEnumerable<string> roles, bool isMember, CancellationToken cancellationToken);
}
