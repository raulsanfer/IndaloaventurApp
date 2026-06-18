namespace IndaloAventurApi.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    Task<(bool Succeeded, Guid? UserId, IEnumerable<string> Roles, bool IsMember)> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);
    Task<(bool Succeeded, Guid? UserId, string? Email, IEnumerable<string> Roles, bool IsMember, IEnumerable<string> Errors)> ValidateSocialLoginAsync(string provider, string token, CancellationToken cancellationToken);
    Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken);
    Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, IEnumerable<string> roles, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ManagedUserDto>> ListUsersAsync(string? email, CancellationToken cancellationToken);
    Task<(bool Succeeded, IEnumerable<string> Errors)> SetIsMemberAsync(Guid userId, bool isMember, CancellationToken cancellationToken);
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserAsync(Guid userId, string email, bool isMember, IEnumerable<string> roles, CancellationToken cancellationToken);
    Task<(bool Succeeded, IEnumerable<string> Errors)> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<(bool Succeeded, IEnumerable<string> Errors)> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken);
    Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);
}
