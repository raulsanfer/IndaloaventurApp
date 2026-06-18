namespace IndaloAventurApi.Application.Abstractions.Security;

public interface IJwtTokenService
{
    string CreateToken(Guid userId, string email, IEnumerable<string> roles, bool isMember);
}
